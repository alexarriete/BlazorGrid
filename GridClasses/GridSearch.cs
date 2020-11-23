using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace BlazorGrid.GridClasses
{
    public enum PropertyType
    {
        noType,
        text,
        number,
        datetime,
        list,
        image
    }

    public enum NumberSelectionType
    {
        Equals,
        Greater_Than,
        Less_Than,
        Between
    }
    public class GridSearch
    {
        public bool DialogOpen { get; set; }
        public string SearchText { get; set; }
        public string SearchText2 { get; set; }
        public string SearchPropName { get; set; }
        public PropertyType SearchPropType { get; set; }
        public DateTime SearchDateFrom { get; set; }
        public DateTime SearchDateTo { get; set; }
        public PropertyInfo SearchProperty { get; set; }
        public List<string> SearchSelect { get; set; }
        public bool CaseSensitive { get; set; }
        public NumberSelectionType NumberSearchTypeSelected { get; set; }

        public async Task CreateGridSearch(GridColumnBase gridColumn, List<string> options)
        {
            SearchSelect = options;
            SearchProperty = gridColumn.PropertyInfo;
            SearchPropName = gridColumn.Name;
            SearchText = "";
            SearchPropType = await Task.Run(()=> GetSearchPropertyType(gridColumn));
            DialogOpen = true;
        }

        public void ResetSearchAsync()
        {
            SearchProperty = null;
            SearchPropName = null;
            SearchText = null;
            SearchText2 = null;
            NumberSearchTypeSelected = NumberSelectionType.Equals;
            SearchPropType = PropertyType.noType;
        }

        private PropertyType GetSearchPropertyType(GridColumnBase gridColumn)
        {           
            if (SearchProperty != null)
            {
                PropertyType searchType = gridColumn.PropertyType;
                if (searchType != PropertyType.datetime && SearchSelect != null && SearchSelect.Any())
                {
                    SearchText = SearchSelect.OrderBy(x => x).FirstOrDefault();
                    searchType = PropertyType.list;
                }
                return searchType;
            }
            return PropertyType.noType;
        }

       

        public async Task<IEnumerable<object>> GetFilteredByDateTimeInterval(IEnumerable<object> objects)
        {
            DateTime result = DateTime.MinValue;
            return await Task.Run(() => objects.Where(n => (SearchProperty == null ? true
                     : SearchProperty.GetValue(n, null) == null)
                     || (!DateTime.TryParse(SearchProperty.GetValue(n, null).ToString(), out result) ? false
                     : Convert.ToDateTime(SearchProperty.GetValue(n, null)) > SearchDateFrom)
                        && (Convert.ToDateTime(SearchProperty.GetValue(n, null)) <= SearchDateTo)));
        }

        public async Task<IEnumerable<object>> GetEqualsValues(IEnumerable<object> objects)
        {
            return await Task.Run(()=> objects.Where(n => SearchProperty == null ? true
                        : SearchProperty.GetValue(n, null) == null ? false
                        : (SearchProperty.GetValue(n, null).ToString() == SearchText)));
        }

        public async Task<IEnumerable<object>> GetGreaterThanValues(IEnumerable<object> objects)
        {
            return await Task.Run(() => objects.Where(n => SearchProperty == null ? true
                         : SearchProperty.GetValue(n, null) == null ? false
                         : (double.Parse(SearchProperty.GetValue(n, null).ToString()) > double.Parse(SearchText))));
        }

        public async Task<IEnumerable<object>> GetLessThanValues(IEnumerable<object> objects)
        {
            return await Task.Run(() => objects.Where(n => SearchProperty == null ? true
                         : SearchProperty.GetValue(n, null) == null ? false
                         : (double.Parse(SearchProperty.GetValue(n, null).ToString()) < double.Parse(SearchText))));
        }

        public async Task<IEnumerable<object>> GetBetweenValues(IEnumerable<object> objects)
        {
            return await Task.Run(() => objects.Where(n => SearchProperty == null ? true
                         : SearchProperty.GetValue(n, null) == null ? false
                          : ( (double.Parse(SearchProperty.GetValue(n, null).ToString()) > double.Parse(SearchText)) 
                               && double.Parse(SearchProperty.GetValue(n, null).ToString()) < double.Parse(SearchText2))));
        }

        public async Task<IEnumerable<object>> GetTextContains(IEnumerable<object> objects)
        {
            SearchText = SearchText.RemoveDiacritics(!CaseSensitive);
            return  await Task.Run(()=> objects.Where(n => SearchProperty == null ? true
                       : SearchProperty.GetValue(n, null) == null ? false
                       : (SearchProperty.GetValue(n, null).ToString() ?? "").RemoveDiacritics(!CaseSensitive).Contains(SearchText)));
        }
       
    }
}
