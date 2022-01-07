using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;


namespace GadjIT.ClientAPI.Repository.GeneralAccess 
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

        public async Task<IEnumerable<T>> LoadData<T, U>(
            string sql,
            string connectionId = "GadjIT_API")
        {
            using IDbConnection connection = new SqlConnection(configuration.GetConnectionString(connectionId));

            return await connection.QueryAsync<T>(sql);
        }
    }
}