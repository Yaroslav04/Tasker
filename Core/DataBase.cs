using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tasker.Model;
using SQLite;
using System.Diagnostics;

namespace Tasker.Core
{
    public class DataBase
    {
        readonly SQLiteAsyncConnection connection;

        public DataBase(string _connectionString)
        {
            connection = new SQLiteAsyncConnection(_connectionString);
            connection.CreateTableAsync<Tasks>().Wait();
        }

        public Task<int> SaveAsync(Tasks _tasks)
        {
            try
            {
                return connection.InsertAsync(_tasks);
                
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public Task<int> UpdateAsync(Tasks _tasks)
        {
            try
            {
                return connection.UpdateAsync(_tasks);
            }
            catch
            {
                return null;
            }

        }

        public Task<int> DeleteAsync(Tasks _tasks)
        {
            try
            {
                return connection.DeleteAsync(_tasks);
            }
            catch
            {
                return null;
            }

        }

        public async Task DeleteAsync(int _id)
        {
            try
            {
                var _tasks = await GetAsync(_id);
                await connection.DeleteAsync(_tasks);
                return;

            }
            catch
            {
                return;
            }

        }

        public Task<List<Tasks>> GetAsync()
        {
            return connection.Table<Tasks>().ToListAsync();
        }

        public Task<Tasks> GetAsync(int _id)
        {
            return connection.Table<Tasks>().Where(x => x.Id == _id).FirstOrDefaultAsync();
        }
    }
}
