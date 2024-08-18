using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using ClosedXML.Excel;
using Contact.Worker.Models;

namespace Contact.Worker;

public class Worker(ILogger<Worker> logger, QueueServiceClient queueServiceClient, BlobServiceClient blobServiceClient, IServiceScopeFactory serviceScopeFactory) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await using var scope = serviceScopeFactory.CreateAsyncScope();
        var contactContext = scope.ServiceProvider.GetRequiredService<ContactContext>();
        var docsContainer = blobServiceClient.GetBlobContainerClient("fileuploads");
        var queueClient = queueServiceClient.GetQueueClient("contacts");
        await queueClient.CreateIfNotExistsAsync(cancellationToken: stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            QueueMessage[] messages = await queueClient.ReceiveMessagesAsync(maxMessages: 10, cancellationToken: stoppingToken);

            foreach (var message in messages)
            {
                var fileKey = message.MessageText;
                logger.LogInformation(
                    "Message from queue: {Message}", fileKey);

                await queueClient.DeleteMessageAsync(
                    message.MessageId,
                    message.PopReceipt,
                    cancellationToken: stoppingToken);

                var contactModelList = await GetContactModelList(docsContainer, fileKey, stoppingToken);

                await contactContext.Contacts.AddRangeAsync(contactModelList);
                await contactContext.SaveChangesAsync();

            }

            await Task.Delay(5000, stoppingToken);
        }
    }

    private static async Task<List<ContactModel>> GetContactModelList(BlobContainerClient blobContainerClient, string fileKey, CancellationToken cancellationToken)
    {
        var contactModelList = new List<ContactModel>();
        var blobClient = blobContainerClient.GetBlobClient(fileKey);
        using BlobDownloadStreamingResult file = await blobClient.DownloadStreamingAsync(cancellationToken: cancellationToken);
        using var stream = file.Content;
        using var workbook = new XLWorkbook(stream);
        var worksheet = workbook.Worksheet(1);
        var rowCount = worksheet.LastRowUsed().RowNumber();
        for (int row = 2; row <= rowCount; row++)
        {
            var contactModel = new ContactModel
            {
                Email = worksheet.Cell(row, 1).GetString(),
                Name = worksheet.Cell(row, 2).GetString(),
            };
            contactModelList.Add(contactModel);
        }

        return contactModelList;
    }
}
