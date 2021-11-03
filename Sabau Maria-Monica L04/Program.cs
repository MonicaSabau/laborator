using System;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Models;

namespace L04
{
    class Program
    {
        private static CloudTableClient tableClient;

        private static CloudTable studentsTable;

        static void Main(string[] args)
        {
            Task.Run(async () => { await Initialize(); })
            .GetAwaiter()
            .GetResult();
        }

        static async Task Initialize()
        {
            string storageConnectionString = "DefaultEndpointsProtocol=https;AccountName=tema403nov;AccountKey=ydK6+4MzD6EKpZXHResiP4Gls8JLMlNySy/T5MsMDuRCyCGNqDfAPWqdncDsm/NkMvsE7IvUMwtKxjzG4frXxQ==;EndpointSuffix=core.windows.net";
            var account = CloudStorageAccount.Parse(storageConnectionString);
            tableClient = account.CreateCloudTableClient();

            studentsTable = tableClient.GetTableReference("studenti");

            await studentsTable.CreateIfNotExistsAsync();

            await AddNewStudent();
            await GetAllStudents();
        }

        private static async Task GetAllStudents()
        {
            Console.WriteLine("Universitate\tCNP\tNume\tEmail\tNumar telefon\tAn");
            TableQuery<StudentEntity> query = new TableQuery<StudentEntity>();

            TableContinuationToken token = null;
            do
            {
                TableQuerySegment<StudentEntity> resultSegment = await studentsTable.ExecuteQuerySegmentedAsync(query, token);
                token = resultSegment.ContinuationToken;

                foreach (StudentEntity entity in resultSegment.Results)
                {
                    Console.WriteLine("{0}\t{1}\t{2} {3}\t{4}\t{5}\t{6}", entity.PartitionKey, entity.RowKey, entity.FirstName, entity.LastName,
                    entity.Email, entity.PhoneNumber, entity.Year);
                }
            } while (token != null);
        }

        private static async Task AddNewStudent()
        {
            var student = new StudentEntity("UVT", "1990903450087");

            student.FirstName = "Walter";
            student.LastName = "Muliz";
            student.Email = "w.muliz@gmail.com";
            student.Year = 4;
            student.PhoneNumber = "0722567987";
            student.Faculty = "AC";

            var insertOperation = TableOperation.Insert(student);

            await studentsTable.ExecuteAsync(insertOperation);
        }
    }
}
