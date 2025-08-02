using AqbaApp.Model.OkdeskEntities;
using System.Threading.Tasks;
using AqbaApp.Model.Client;
using System.Collections.Generic;
using AqbaApp.Core.Api;
using AqbaApp.Service.Client;
using System.Linq;

namespace AqbaApp.Service.OkdeskEntity
{
    public class CompanyService(SettingService<MainSettings> mainSettings, Immutable immutable, GetItemService request, CompanyCategoryService categoryService)
    {
        public async Task<List<Company>?> GetCompaniesFromCloudApi(long startIndex = 0, long limit = 50)
        {
            List<Category>? categories = await categoryService.GetCategories();

            if (categories == null || categories.Count == 0)
                return null;

            List<Company>? companies = [];

            foreach (var category in categories)
            {
                string link = $"{mainSettings.Settings.ServerAddress}/{immutable.ApiMainEndpoint}/company/by_category?categorycode={category.Code}";
                startIndex = 0;

                while (true)
                {
                    string linkWithIndex = link + "&startIndex=" + startIndex;

                    List<Company>? companiesFromApi = await request.GetRangeOfItems<Company>(linkWithIndex);

                    if (companiesFromApi == null || companiesFromApi.Count == 0) break;

                    foreach (var company in companiesFromApi)
                    {
                        company.Category = category;
                        companies.Add(company);
                    }

                    startIndex = companies.Last().Id + 1;

                    if (companiesFromApi.Count < limit) break;
                }

                /*await foreach (List<Company> companiesFromCloudApi in request.GetAllItems<Company>(link, startIndex, limit))
                {
                    foreach (var company in companiesFromCloudApi)
                    {
                        company.Category = category;
                        companies.Add(company);
                    }
                }*/
            }

            return companies;
        }
    }
}
