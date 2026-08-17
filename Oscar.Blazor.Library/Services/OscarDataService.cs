using System;
using System.Data;
using System.Dynamic;
using System.Linq.Dynamic.Core;
using System.Text.Encodings.Web;
using AutoMapper;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Oscar.Blazor.Library.Controllers;
using Oscar.Core.DTOs;
using Oscar.Core.Entities;
using Oscar.Data.Context;

namespace Oscar.Blazor.Library.Services
{
    public class OscarDataService
    {
        #region - Private Declarations -
        private readonly OscarContext _context;
        private readonly NavigationManager _navigationManager;
        private readonly IMapper _mapper;
        #endregion

        #region - Constructor -
        public OscarDataService(OscarContext context, NavigationManager navigationManager, IMapper mapper)
        {
            _context = context;
            _navigationManager = navigationManager;
            _mapper = mapper;
        }
        #endregion

        #region - Properties -
        protected OscarContext Context => _context;
        #endregion

        #region - Public Methos -

        public void Reset() => Context.ChangeTracker.Entries().Where(e => e.Entity != null).ToList().ForEach(e => e.State = EntityState.Detached);

        public void ApplyQuery<T>(ref IQueryable<T> items, Query query = null)
        {
            if (query == null) return;

            if (!string.IsNullOrEmpty(query.Filter))
            {
                if (query.FilterParameters != null)
                {
                    items = items.Where(query.Filter, query.FilterParameters);
                }
                else
                {
                    items = items.Where(query.Filter);
                }
            }

            if (!string.IsNullOrEmpty(query.OrderBy))
            {
                items = items.OrderBy(query.OrderBy);
            }

            if (query.Skip.HasValue)
            {
                items = items.Skip(query.Skip.Value);
            }

            if (query.Top.HasValue)
            {
                items = items.Take(query.Top.Value);
            }
        }

        public async Task CallApiExport(string path, Query? query = null, bool forceLoad= true)
        {
            _navigationManager.NavigateTo(query != null ? query.ToUrl(path) : path, forceLoad);
            await Task.CompletedTask;
        }
        #endregion

        #region - Actors -
        public virtual void OnActorsRead(ref IQueryable<Actor> items) { }
        public virtual void OnActorGet(Actor item) { }
        public virtual void OnGetActorById(ref IQueryable<Actor> items) { }

        public async Task<IQueryable<ActorDto>> GetActors(Query query = null)
        {
            var items = Context.Actors.AsQueryable();

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach (var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnActorsRead(ref items);

            return await Task.FromResult(items.Select(x => _mapper.Map<ActorDto>(x)));
        }

        public async Task<ActorDto> GetActorById(int id)
        {
            var items = Context.Actors.AsNoTracking().Where(i => i.Id == id);

            OnGetActorById(ref items);

            var itemToReturn = items.FirstOrDefault();
            if (itemToReturn != null)
                OnActorGet(itemToReturn);
            return await Task.FromResult(_mapper.Map<ActorDto>(itemToReturn));
        }

        public async Task<FileStreamResult> ExportActors(ExportType exportType, Query query = null, string fileName = null)
        {
            //string reportUrl = $"export/oscardata/actors/{exportType.ToString().ToLowerInvariant()}(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')";
            //await CallApiExport(reportUrl, query);
            var task = await GetActors(query);
            var items = await task.ToListAsync();
            if (exportType == ExportType.Excel)
                return ExportUtil.ToExcel(items, fileName);

            return ExportUtil.ToCsv(items, fileName);
        }
        #endregion

        #region - Countries -

        public async Task<IQueryable<CountryDto>> GetCountries(Query query = null)
        {
            var items = Context.Countries.AsQueryable();

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach (var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            return await Task.FromResult(items.Select(x => _mapper.Map<CountryDto>(x)));
        }

        public async Task<FileStreamResult> ExportCountries(ExportType exportType, Query query = null, string fileName = null)
        {
            var task = await GetCountries(query);
            var items = await task.ToListAsync();
            if (exportType == ExportType.Excel)
                return ExportUtil.ToExcel(items, fileName);

            return ExportUtil.ToCsv(items, fileName);
        }

        #endregion

        #region - Client Catalogues -

        public virtual void OnClientCataloguesDetailsRead(ref IQueryable<ClientCataloguesDetail> items) { }

        public async Task<IQueryable<ClientCataloguesDetail>> GetClientCataloguesDetails(Query query = null)
        {
            var items = Context.ClientCataloguesDetails.AsQueryable();

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach (var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnClientCataloguesDetailsRead(ref items);

            return await Task.FromResult(items);
        }
        public async Task<FileStreamResult> ExportClientCataloguesDetails(ExportType exportType, Query query = null, string fileName = null)
        {
            //string reportUrl = $"export/oscardata/clientcataloguesdetails/{exportType.ToString().ToLowerInvariant()}(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')";
            //await CallApiExport(reportUrl, query);
            var task = await GetClientCataloguesDetails(query);
            var items = await task.ToListAsync();
            if (exportType == ExportType.Excel)
                return ExportUtil.ToExcel(items, fileName);

            return ExportUtil.ToCsv(items, fileName);
        }

        #endregion

        #region - Catalogues Society Works -
        public async Task<int> GetClientCatalogueSocietyWorks(int? ClientId, int? CatalogueId, int? SocietyId, bool? IncludePreviouslyRegistered)
        {
            OnGetClientCatalogueSocietyWorksDefaultParams(ref ClientId, ref CatalogueId, ref SocietyId, ref IncludePreviouslyRegistered);

            SqlParameter[] @params =
            {
                new SqlParameter("@returnVal", SqlDbType.Int) {Direction = ParameterDirection.Output},
                new SqlParameter("@ClientId", SqlDbType.Int, -1) {Direction = ParameterDirection.Input, Value = ClientId},
                new SqlParameter("@CatalogueId", SqlDbType.Int, -1) {Direction = ParameterDirection.Input, Value = CatalogueId},
                new SqlParameter("@SocietyId", SqlDbType.Int, -1) {Direction = ParameterDirection.Input, Value = SocietyId},
                new SqlParameter("@IncludePreviouslyRegistered", SqlDbType.Bit, -1) {Direction = ParameterDirection.Input, Value = IncludePreviouslyRegistered},
            };

            foreach (var _p in @params)
            {
                if ((_p.Direction == ParameterDirection.Input || _p.Direction == ParameterDirection.InputOutput) && _p.Value == null)
                {
                    _p.Value = DBNull.Value;
                }
            }

            Context.Database.ExecuteSqlRaw("EXEC @returnVal=[dbo].[sp_GetClientCatalogueSocietyWorks] @ClientId, @CatalogueId, @SocietyId, @IncludePreviouslyRegistered", @params);

            int result = Convert.ToInt32(@params[0].Value);

            OnGetClientCatalogueSocietyWorksInvoke(ref result);

            return await Task.FromResult(result);
        }

        public virtual void OnGetClientCatalogueSocietyWorksDefaultParams(ref int? ClientId, ref int? CatalogueId, ref int? SocietyId, ref bool? IncludePreviouslyRegistered) { }

        public virtual void OnGetClientCatalogueSocietyWorksInvoke(ref int result) { }
        #endregion

        #region - Client Details -

        public virtual void OnClientDetailsRead(ref IQueryable<ClientDetail> items) { }

        public async Task<IQueryable<ClientDetail>> GetClientDetails(Query query = null)
        {
            var items = Context.ClientDetails.AsQueryable();

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach (var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnClientDetailsRead(ref items);

            return await Task.FromResult(items);
        }

        public async Task<FileStreamResult> ExportClientDetails(ExportType exportType, Query query = null, string fileName = null)
        {
            var task = await GetClientDetails(query);
            var items = await task.ToListAsync();
            if (exportType == ExportType.Excel)
                return ExportUtil.ToExcel(items, fileName);

            return ExportUtil.ToCsv(items, fileName);
        }
        #endregion

        #region - Client Works List -

        public async Task<IQueryable<ClientWorkItem>> GetClientWorksList(Query query = null)
        {
            var items = Context.ClientWorkItems.AsQueryable();

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach (var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            return await Task.FromResult(items);
        }
        public async Task<FileStreamResult> ExportClientWorksList(ExportType exportType, Query query = null, string fileName = null)
        {
            var task = await GetClientWorksList(query);
            var items = await task.ToListAsync();
            if (exportType == ExportType.Excel)
                return ExportUtil.ToExcel(items, fileName);

            return ExportUtil.ToCsv(items, fileName);
        }

        #endregion

        #region - Production Countries List -

        public async Task<IQueryable<ProductionCountryItem>> GetProductionCountriesItems(Query query = null)
        {
            var items = Context.ProductionCountryItems.AsQueryable();

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach (var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            return await Task.FromResult(items);
        }
        public async Task<FileStreamResult> ExportProductionCountryItems(ExportType exportType, Query query = null, string fileName = null)
        {
            var task = await GetProductionCountriesItems(query);
            var items = await task.ToListAsync();
            if (exportType == ExportType.Excel)
                return ExportUtil.ToExcel(items, fileName);

            return ExportUtil.ToCsv(items, fileName);
        }

        #endregion

        #region - Client Work Rights List -

        public async Task<IQueryable<ClientWorkRightItem>> GetClientWorkRightItems(Query query = null)
        {
            var items = Context.ClientWorkRightItems.AsQueryable();

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach (var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            return await Task.FromResult(items);
        }
        public async Task<FileStreamResult> ExportClientWorkRightItems(ExportType exportType, Query query = null, string fileName = null)
        {
            var task = await GetClientWorkRightItems(query);
            var items = await task.ToListAsync();
            if (exportType == ExportType.Excel)
                return ExportUtil.ToExcel(items, fileName);

            return ExportUtil.ToCsv(items, fileName);
        }

        #endregion

        #region - Client Client Stat Item -

        public virtual void OnClientYearlyStatRead<T>(ref IQueryable<T> items) { }

        public async Task<IQueryable<ClientWorkStatItem>> GetClientYearlyStats(Query query = null)
        {
            var items = Context.ClientWorkYearlyStats.AsQueryable();

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach (var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnClientYearlyStatRead(ref items);

            return await Task.FromResult(items);
        }

        public async Task<IQueryable<ClientWorkStatItemEx>> GetClientProductionYearlyStats(Query query = null)
        {
            var items = Context.ClientWorkProductionYearlyStats.AsQueryable();

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach (var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnClientYearlyStatRead(ref items);

            return await Task.FromResult(items);
        }


        #endregion


        public async Task<List<DynamicEntityItem>> GetClientWorkYearlyStats(bool isProductionYear, int? currentYear, int? clientId)
        {
            OnGetClientWorkYearlyStatsDefaultParams(ref currentYear, ref clientId);

            var currentYearParam = new SqlParameter("@CurrentYear", currentYear);
            var clientIdParam = new SqlParameter("@ClientId", clientId.HasValue ? clientId.Value : (object)DBNull.Value);

            var sql = $"EXEC sp_GetClientWork{(isProductionYear? "Production" : "")}YearlyStats @CurrentYear, @ClientId";

            var result = new List<DynamicEntityItem>();

            using (var command = this.Context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = sql;
                command.CommandType = System.Data.CommandType.Text;
                command.Parameters.Add(currentYearParam);
                command.Parameters.Add(clientIdParam);

                await Context.Database.OpenConnectionAsync();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var dynamicRow = new DynamicEntityItem();

                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            dynamicRow.Properties[reader.GetName(i)] = reader.GetValue(i);
                        }

                        result.Add(dynamicRow);
                    }
                }
            }
            OnGetClientWorkYearlyStatsInvoke(ref result);
            return result;
        }

        public async Task<FileStreamResult> ExportClientWorkYearlyStats(ExportType exportType, bool isProductionYear, int currentYear, int? clientId, string fileName = null)
        {
            var items = await GetClientWorkYearlyStats(isProductionYear, currentYear, clientId);
            if (exportType == ExportType.Excel)
                return ExportUtil.ToExcelDynamic(items, fileName);

            return ExportUtil.ToCsvDynamic(items, fileName);
        }
        public virtual void OnGetClientWorkYearlyStatsDefaultParams(ref int? currentYear, ref int? clientId) { }

        public virtual void OnGetClientWorkYearlyStatsInvoke(ref List<DynamicEntityItem> result) { }
    }

    public enum ExportType
    {
        Excel,
        Csv
    }


    

}
