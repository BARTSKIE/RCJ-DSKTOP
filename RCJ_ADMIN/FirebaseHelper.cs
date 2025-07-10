using Firebase.Database;
using Firebase.Database.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RCJ_ADMIN
{
    public static class FirebaseHelper
    {
        private static readonly FirebaseClient firebase = new FirebaseClient("https://rcj-optical-site-default-rtdb.firebaseio.com/");

        // Delete a Transaction
        public static async Task DeleteTransaction(string id)
        {
            await firebase
                .Child("transactions")
                .Child(id)
                .DeleteAsync();
        }




        // ADD a transaction
        // Add Transaction (with ID saved)
        public static async Task AddTransaction(TransactionsPage.Transaction t)
        {
            var response = await firebase
                .Child("transactions")
                .PostAsync(t);

            t.Id = response.Key;

            // Save with the ID included
            await firebase
                .Child("transactions")
                .Child(t.Id)
                .PutAsync(t);
        }

        // Get All Transactions (with ID restored)
        public static async Task<List<TransactionsPage.Transaction>> GetAllTransactions()
        {
            var transactions = await firebase
              .Child("transactions")
              .OnceAsync<TransactionsPage.Transaction>();

            return transactions.Select(item =>
            {
                var obj = item.Object;
                obj.Id = item.Key;
                return obj;
            }).ToList();
        }

    }
}
