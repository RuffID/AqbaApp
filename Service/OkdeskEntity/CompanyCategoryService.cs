using System.Threading.Tasks;
using System.Collections.Generic;
using AqbaApp.Model.OkdeskEntities;
using AqbaApp.Model.Client;
using AqbaApp.Core.Api;
using AqbaApp.Service.Client;

namespace AqbaApp.Service.OkdeskEntity
{
    public class CompanyCategoryService(SettingService<MainSettings> mainSettings, Immutable immutable, GetItemService request)
    {
        public async Task<List<Category>?> GetCategories()
        {
            return await request.GetRangeOfItems<Category>($"{mainSettings.Settings.ServerAddress}/{immutable.ApiMainEndpoint}/category/list");            
        }
    }
}
