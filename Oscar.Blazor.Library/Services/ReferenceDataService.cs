using System.Collections.ObjectModel;
using LinqKit;
using MediatR;
using Oscar.Core.DTOs;
using Oscar.Infrastructure.Features.Actor.Queries;
using Oscar.Infrastructure.Features.Catalogue.Queries;
using Oscar.Infrastructure.Features.Clients.Queries;
using Oscar.Infrastructure.Features.Country.Queries;
using Oscar.Infrastructure.Features.ProductionCompany.Queries;
using Oscar.Infrastructure.Features.Rights.Queries;
using Oscar.Infrastructure.Features.Director.Queries;
using Oscar.Infrastructure.Features.Distributor.Queries;
using Oscar.Infrastructure.Features.Producer.Queries;
using Oscar.Infrastructure.Features.ScreenWriter.Queries;
using Oscar.Infrastructure.Features.ScriptWriter.Queries;
using Oscar.Infrastructure.Features.Works.Queries;
using Oscar.Infrastructure.Features.Channel.Queries;
using Oscar.Infrastructure.Features.Society.Queries;
using Oscar.Infrastructure.Features.CustomServiceManager.Queries;

namespace Oscar.Blazor.Library.Services
{
    public class ReferenceDataService
    {
        #region - Private Declarations -
        private readonly IMediator _mediator;
        private List<ClientDto> _clients = new List<ClientDto>();
        private List<CompanyDto> _companies = new List<CompanyDto>();
        private List<CountryDto> _countries = new List<CountryDto>();
        private List<PersonDto> _actors = new List<PersonDto>();
        private List<PersonDto> _directors = new List<PersonDto>();
        private List<PersonDto> _producers = new List<PersonDto>();
        private List<PersonDto> _distributors = new List<PersonDto>();
        private List<PersonDto> _screenWriters = new List<PersonDto>();
        private List<PersonDto> _scriptWriters = new List<PersonDto>();
        private static List<RightsTypeDto> _rightsType = new List<RightsTypeDto>();
        private List<CountriesGroupsDto> _countryGroups = new List<CountriesGroupsDto>();
        private List<ChannelDto> _allChannels = new List<ChannelDto>();
        private List<LanguageDto> _allLanguages = new List<LanguageDto>();
        private List<SocietyDto> _societies = new List<SocietyDto>();
        private List<OperatorDto> _operators = new List<OperatorDto>();
        #endregion

        #region - Constructor -
        public ReferenceDataService(IMediator mediator)
        {
            _mediator = mediator;
        }
        #endregion

        #region - Collections -
        public async Task<List<PersonDto>> GetActors()
        {
            await LoadActorsAsync();
            return _actors;
        }

        public async Task<List<PersonDto>> GetDirectors()
        {
            await LoadDirectorsAsync();
            return _directors;
        }
        public async Task<List<PersonDto>> GetDistributors()
        {
            await LoadDistributorsAsync();
            return _distributors;
        }

        public async Task<List<PersonDto>> GetScreenWriters()
        {
            await LoadScreenWritersAsync();
            return _screenWriters;
        }
        public async Task<List<PersonDto>> GetScriptWriters()
        {
            await LoadScriptWritersAsync();
            return _scriptWriters;
        }
        public async Task<List<PersonDto>> GetProducers()
        {
            await LoadProducersAsync();
            return _producers;
        }

        public async Task<List<ClientDto>> GetClients()
        {
            await LoadClientsAsync();
            return _clients;
        }

        public async Task<List<CompanyDto>> GetCompanies()
        {
            await LoadCompaniesAsync();
            return _companies;
        }

        public async Task<List<CountryDto>> GetCountries()
        {
            await LoadCountriesAsync();
            return _countries;
        }

        public async Task<List<CountriesGroupsDto>> GetCountryGroups()
        {
            await LoadCountryGroups();
            return _countryGroups;
        }

        public async Task<List<RightsTypeDto>> GetRightsType()
        {
            await LoadRightsTypeAsync();
            return _rightsType;
        }

        public async Task<List<ChannelDto>> GetAllChannels()
        {
            await LoadAllChannels();
            return _allChannels;
        }

        public async Task<List<LanguageDto>> GetAllLanguages()
        {
            await LoadAllLanguages();
            return _allLanguages;
        }

        public async Task<List<SocietyDto>> GetSocieties()
        {
            await LoadSocieties();
            return _societies;
        }

        public async Task<List<OperatorDto>> GetOperators()
        {
            await LoadOperators();
            return _operators;
        }
        #endregion

        #region - Public Load Methods -
        public async Task LoadActorsAsync(bool force = false)
        {
            if (!force && _actors.Any()) return;
            var actors = await _mediator.Send(new GetAllActorsQuery());
            _actors = actors.Value.OrderBy(x => x.LastName).ToList();
        }

        public async Task LoadClientsAsync(bool force = false)
        {
            if (!force && _clients.Any()) return;
            var clients = await _mediator.Send(new GetAllClientsQuery());
            _clients = clients.Value.OrderBy(x => x.ClientName).ToList();
        }

        public async Task LoadCountriesAsync(bool force = false)
        {
            if (!force && _countries.Any()) return;
            var countries = await _mediator.Send(new GetAllCountriesQuery());
            _countries = countries.Value.OrderBy(x => x.Name).ToList();
        }

        public async Task LoadCompaniesAsync(bool force = false)
        {
            if (!force && _companies.Any()) return;
            var companies = await _mediator.Send(new GetAllCompaniesQuery());
            _companies = companies.Value.OrderBy(x => x.Name).ToList();
        }

        public async Task LoadCountryGroups(bool force = false)
        {
            if (!force && _companies.Any()) return;
            var companyGroups = await _mediator.Send(new GetAllCountriesGroupsQuery());
            _countryGroups = companyGroups.Value.OrderBy(x => x.Name).ToList();
        }

        public async Task LoadDirectorsAsync(bool force = false)
        {
            if (!force && _directors.Any()) return;
            var directors = await _mediator.Send(new GetAllDirectorsQuery());
            _directors = directors.Value.OrderBy(x => x.LastName).ToList();
        }

        public async Task LoadDistributorsAsync(bool force = false)
        {
            if (!force && _distributors.Any()) return;
            var distributors = await _mediator.Send(new GetAllDistributorsQuery());
            _distributors = distributors.Value.OrderBy(x => x.LastName).ToList();
        }

        public async Task LoadProducersAsync(bool force = false)
        {
            if (!force && _producers.Any()) return;
            var producers = await _mediator.Send(new GetAllProducersQuery());
            _producers = producers.Value.OrderBy(x => x.LastName).ToList();
        }

        public async Task LoadScreenWritersAsync(bool force = false)
        {
            if (!force && _screenWriters.Any()) return;
            var items = await _mediator.Send(new GetAllScreenWritersQuery());
            _screenWriters = items.Value.OrderBy(x => x.LastName).ToList();
        }

        public async Task LoadScriptWritersAsync(bool force = false)
        {
            if (!force && _scriptWriters.Any()) return;
            var items = await _mediator.Send(new GetAllScriptWritersQuery());
            _scriptWriters = items.Value.OrderBy(x => x.LastName).ToList();
        }

        public async Task LoadRightsTypeAsync(bool force = false)
        {
            if (!force && _rightsType.Any()) return;
            var rights = await _mediator.Send(new GetRightsTypeQuery());
            _rightsType = rights.Value.OrderBy(x => x.Id).ToList();
        }

        public async Task LoadAllChannels(bool force = false)
        {
            if (!force && _allChannels.Any()) return;
            var channels = await _mediator.Send(new GetAllChannelsQuery());
            _allChannels = channels.Value.OrderBy(x => x.Name).ToList();
        }

        public async Task LoadAllLanguages(bool force = false)
        {
            if (!force && _allLanguages.Any()) return;
            var languages = await _mediator.Send(new GetLanguageStaticDataQuery());
            _allLanguages = languages.Value.OrderBy(x => x.Name).ToList();
        }

        public async Task LoadSocieties(bool force = false)
        {
            if (!force && _societies.Any()) return;
            var societies = await _mediator.Send(new GetAllSocietiesQuery());
            _societies = societies.Value.OrderBy(x => x.Name).ToList();
        }

        public async Task LoadOperators(bool force = false)
        {
            if (!force && _operators.Any()) return;
            var operators = await _mediator.Send(new GetAllOperatorsQuery());
            _operators = operators.Value.OrderBy(x => x.FullName).ToList();
        }
        #endregion

        #region - Empty Methods -
        public List<T> Empty<T>() => new List<T>();

        #endregion

        #region - Get Methods -
        public ClientDto? GetClient(int clientId)
        {
            var request = new GetClientByIdQuery() { Id = clientId };
            var task = Task.Run(() => _mediator.Send(request));
            task.Wait();
            if (!task.IsCanceled && !task.IsFaulted && task.Result?.Value != null)
            {
                return task.Result.Value;
            }

            return null;
        }
        public void GetClientCatalogs(int clientId, Action<IEnumerable<CatalogueDto>> callback)
        {
            var request = new GetCataloguesQuery() { ClientID = clientId };
            var task = Task.Run(() => _mediator.Send(request));
            task.Wait();
            if (task.IsCanceled || task.IsFaulted) return;
            if (task.Result?.Value != null)
            {
                callback(task.Result.Value);
            }
        }
        public CountryDto? GetCountry(int countryId)
        {
            var request = new GetCountryByIdQuery() { Id = countryId };
            var task = Task.Run(() => _mediator.Send(request));
            task.Wait();
            if (!task.IsCanceled && !task.IsFaulted && task.Result?.Value != null)
            {
                return task.Result.Value.Records.FirstOrDefault();
            }

            return null;
        }
        public CompanyDto? GetCompany(int companyId)
        {
            var task = Task.Run(() => SearchCompanies(companyId, null));
            task.Wait();
            if (!task.IsCanceled && !task.IsFaulted)
            {
                return task.Result.FirstOrDefault();
            }

            return null;
        }
        #endregion

        #region - Search Methods -

        public async Task<IEnumerable<WorksTitleResponseDto>> SearchTitles(string title)
        {
            var result = await _mediator.Send(new GetWorksTitleAutoCompleteQuery { Title = title });
            if (result is { IsSuccess: true })
            {
                return result.Value;
            }

            return Empty<WorksTitleResponseDto>();
        }

        public async Task<IEnumerable<ClientDto>> SearchClients(int? id, string? name)
        {
            var request = new GetClientsQuery{ BaseEntityName = "Clients" };
            if (!string.IsNullOrWhiteSpace(name))
            {
                request.SearchObjects.Add(new SearchObject("Clients", "string", "ClientName", name));
            }
            if (id.HasValue)
            {
                request.SearchObjects.Add(new SearchObject("Clients", "number", "Id", $"{id}"));
            }
            var result = await _mediator.Send(request);
            if (result is { IsSuccess: true })
            {
                return result.Value.Records;
            }

            return Empty<ClientDto>();
        }

        public async Task<IEnumerable<CompanyDto>> SearchCompanies(int? id, string? name)
        {
            var request = new GetCompanyQuery() { BaseEntityName = "Company" };
            if (!string.IsNullOrWhiteSpace(name))
            {
                request.SearchObjects.Add(new SearchObject("Company", "string", "Name", name));
            }
            if (id.HasValue)
            {
                request.SearchObjects.Add(new SearchObject("Company", "number", "Id", $"{id}"));
            }
            var result = await _mediator.Send(request);
            if (result is { IsSuccess: true })
            {
                return result.Value.Records;
            }

            return Empty<CompanyDto>();
        }

        public async Task SearchAllCompanies(Predicate<CompanyDto> predicate, Action<IEnumerable<CompanyDto>> callback)
        {
            await Search<CompanyDto>(GetCompanies, predicate, callback);
        }

        public async Task<IEnumerable<CountryDto>> SearchCountries(string searchTerm)
        {
            var request = new GetCountryQuery();
            request.SearchObjects.Add(new SearchObject("Country", "string", "Name", searchTerm));
            var result = await _mediator.Send(request);
            if (result is { IsSuccess: true })
            {
                return result.Value.Records;
            }

            return new List<CountryDto>();
        }

        public async Task<IEnumerable<CatalogueDto>> SearchCatalogs(int? id, string? name)
        {
            var request = new GetCatalogueQuery() { BaseEntityName = "Catalogue" };
            if (id.HasValue)
            {
                request.SearchObjects.Add(new SearchObject("Catalogue", "number", "ClientId", $"{id}"));
            }
            if (!string.IsNullOrWhiteSpace(name))
            {
                request.SearchObjects.Add(new SearchObject("Catalogue", "string", "Name", $"{name}"));
            }
            var result = await _mediator.Send(request);
            if (result is { IsSuccess: true })
            {
                return result.Value.Records;
            }

            return new List<CatalogueDto>();
        }

        public async Task SearchRightsType(Predicate<RightsTypeDto> predicate, Action<IEnumerable<RightsTypeDto>> callback)
        {
            await Search<RightsTypeDto>(GetRightsType, predicate, callback);
        }

        public async Task SearchActors(Predicate<PersonDto> predicate, Action<IEnumerable<PersonDto>> callback)
        {
            await Search<PersonDto>(GetActors, predicate, callback);
        }

        public async Task SearchDirectors(Predicate<PersonDto> predicate, Action<IEnumerable<PersonDto>> callback)
        {
            await Search<PersonDto>(GetDirectors, predicate, callback);
        }

        public async Task SearchProducers(Predicate<PersonDto> predicate, Action<IEnumerable<PersonDto>> callback)
        {
            await Search<PersonDto>(GetProducers, predicate, callback);
        }

        public async Task Search<T>(Func<Task<List<T>>> loader, Predicate<T> predicate, Action<IEnumerable<T>> callback)
        {
            var items = await loader();
            callback(items.FilterBy(predicate));
        } 
        #endregion
    }

    public static class ListExtensions
    {
        public static ICollection<T> ToCollection<T>(this IEnumerable<T> source)
        {
            var result = new Collection<T>();
            source.ForEach(result.Add);
            return result;
        }
        public static List<T> FilterBy<T>(this List<T> source, Predicate<T> predicate)
        {
            return source.Where(x => predicate(x)).ToList();
        }

        public static void FilterBy<T>(this List<T> source, Predicate<T> predicate, Action<IEnumerable<T>> callback)
        {
            var items = source.Where(x => predicate(x)).ToList();
            callback.Invoke(items);
        }

        public static IEnumerable<IEnumerable<T>> ToChunks<T>(this IEnumerable<T> enumerable, int chunkSize)
        {
            int itemsReturned = 0;
            var list = enumerable.ToList();
            int count = list.Count;
            while (itemsReturned < count)
            {
                int currentChunkSize = Math.Min(chunkSize, count - itemsReturned);
                yield return list.GetRange(itemsReturned, currentChunkSize);
                itemsReturned += currentChunkSize;
            }
        }

        //public Task<List<CatalogueDto>> GetCatalogsById(List<int> catalogsIds)
        //{
        //    var chunks = catalogsIds.ToChunks(500);
        //    return chunks.Select(chunk => DbSet.Where(c => chunk.Contains(c.CatalogId))).SelectMany(x => x).ToList();
        //}
    }
}
