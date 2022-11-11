using Dapper;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data; 
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;


namespace GadjIT_ClientAPI.Repository.GeneralAccess 
{
    public interface IGeneralAccessService
    {

        Task<IEnumerable<T>> LoadDataViaSp<T,U>(
            string storedProcedure,
            U parameters,
            string connectionId = "GadjIT_API");


        Task<IEnumerable<T>> LoadData<T, U>(
            string sql,
            string connectionId = "GadjIT_API");

    }

    public class GeneralAccessService : IGeneralAccessService
    {
        private readonly IConfiguration configuration;

        public GeneralAccessService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<IEnumerable<T>> LoadDataViaSp<T,U>(
            string storedProcedure,
            U parameters,
            string connectionId = "GadjIT_API")
        {
            using IDbConnection connection = new SqlConnection(configuration.GetConnectionString(connectionId));

            return await connection.QueryAsync<T>(
                storedProcedure,
                parameters,
                commandType: CommandType.StoredProcedure);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="connectionId"></param>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <returns></returns>
        public async Task<IEnumerable<T>> LoadData<T, U>(
            string sql,
            string connectionId = "GadjIT_API")
        {
            using IDbConnection connection = new SqlConnection(configuration.GetConnectionString(connectionId));

            return await connection.QueryAsync<T>(sql);
        }
    }
}