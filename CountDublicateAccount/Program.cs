using System.Text;
using CRMConnectAPI.Services;
using CRMEntity.Models.Account;
using Microsoft.Xrm.Sdk.Query;

Console.ReadLine();

try
{
    var apiClient = new ApiClient();
    var account = new Account(apiClient.Service);

    account.GetAll(new ColumnSet(Account.Fields.Inn), out var accountEntities);
    var skip = new List<Guid>();
    int countDuplicate = 0;
    int count = 0;
    var textResult = new StringBuilder();

    foreach (var accountEntity in accountEntities)
    {
        Console.WriteLine(++count);
        if (skip.Contains(accountEntity.Id)) continue;
        string inn = accountEntity.GetAttributeValue<string>(Account.Fields.Inn);
        if (string.IsNullOrEmpty(inn)) continue;

        account.GetAll(inn, null, out var accountInnEntities);

        if (accountInnEntities.Count == 1) continue;

        countDuplicate++;

        string text = $"{countDuplicate} - {string.Join(", ", accountInnEntities.Select(a => a.Id))}";
        skip.AddRange(accountInnEntities.Select(a => a.Id));
        textResult.AppendLine(text);
    }

    using (StreamWriter writer = new StreamWriter("file.txt", false))
    {
        await writer.WriteLineAsync(countDuplicate.ToString());
        await writer.WriteLineAsync(textResult);
    }
}
catch (Exception e)
{
    using (StreamWriter writer = new StreamWriter("error.txt", false))
    {
        await writer.WriteLineAsync(e.ToString());
    }
}



