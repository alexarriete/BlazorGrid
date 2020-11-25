using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace BlazorGrid.GridClasses
{
    public class ContextMenuOption
    {
        public string Name { get; set; }
        public string BtnCss { get; set; }
        public string Page { get; set; }     
        public string Id { get; set; }
    }
    public class CellContexMenuOption
    {
        public GridColumnBase GridColumn { get; set; }       
        public object RowValues { get; set; }
        public List<object> OneItemList { get; set; }       

        public CellContexMenuOption(GridColumnBase gridColumn, object context, string urlFolder)
        {
            GridColumn = gridColumn;
            RowValues = context;
            OneItemList = new List<object>();
            OneItemList.Add(RowValues);
            string keyValue = GridColumnBase.GetKeyValue(context, context.GetType().GetProperty(gridColumn.KeyColumnName)).ToString();
            gridColumn.InitializeContextMenuOptions(keyValue, urlFolder );
        }
        
    }


    public class GridColumnBase
    {
        public string Name { get; set; }        
        public int Position { get; set; }
        public bool ShowKeyColumn { get; set; }
        public bool KeyColumn { get; set; }   
        public string KeyColumnName { get; set; }
        public PropertyInfo PropertyInfo { get; set; }
        public List<ContextMenuOption> ContextMenuOptions { get; set; }
        public CellContexMenuOption SelectedContextMenu { get; set; }
        public string SortSymbol { get; set; }
        public PropertyType PropertyType { get; set; }

        public static object GetKeyValue(object obj, PropertyInfo prop)
        {
            return prop.GetValue(obj, null);
        }
        public GridColumnBase(PropertyInfo prop, int position, string keyColumnName)
        {
            PropertyInfo = prop;
            Position = position;
            KeyColumnName = keyColumnName;
            Name = prop.GetCustomAttribute<DisplayAttribute>()?.Name ?? prop.Name;
            KeyColumn = keyColumnName == prop.Name && !ShowKeyColumn;
            PropertyType = GetPropertyType();
            
        }

        public virtual void InitializeContextMenuOptions(string keyColumnName, string urlFolder)
        {
            ContextMenuOptions = new List<ContextMenuOption>();
            ContextMenuOptions.Add(new ContextMenuOption() {BtnCss= "btn-default", Name = "Details", Page=$"{urlFolder}/detail/{keyColumnName}" });
            ContextMenuOptions.Add(new ContextMenuOption() { BtnCss = "btn-success", Name = "Edit", Page = $"{urlFolder}/edit/{keyColumnName}" });
            ContextMenuOptions.Add(new ContextMenuOption() { BtnCss = "btn-primary", Name = "Validate", Page = $"{urlFolder}/validate/{keyColumnName}" });
            ContextMenuOptions.Add(new ContextMenuOption() { BtnCss = "btn-info", Name = "Create", Page = $"{urlFolder}/create" });
            ContextMenuOptions.Add(new ContextMenuOption() { BtnCss = "btn-danger", Name = "Delete", Page = $"{urlFolder}/delete/{keyColumnName}" });            
        }

        private PropertyType GetPropertyType()
        {
            if (PropertyInfo !=null)
            {
                if (IsNumericType(PropertyInfo))
                {
                    return PropertyType.number;
                }
                string result = PropertyInfo.GetCustomAttributesData()
                    .FirstOrDefault(x => x.AttributeType.Name.Contains("ColumnAttribute"))?
                    .NamedArguments?.FirstOrDefault().TypedValue.Value.ToString() ??
                 PropertyInfo.PropertyType.Name.ToLower();

                result = result == "string" ? "text" : result;
                object resultType = TypeCode.DBNull;
                bool knownType = Enum.TryParse(typeof(TypeCode), result, false, out resultType);
                PropertyType searchType = knownType ? (PropertyType)Enum.Parse(typeof(PropertyType), result): PropertyType.text;
                return searchType;
            }
            return PropertyType.noType;
        }

        private bool IsNumericType(PropertyInfo prop)
        {
            object resultType = TypeCode.DBNull;
            bool knownType  = Enum.TryParse(typeof(TypeCode),  prop.PropertyType.Name, false, out resultType);
            if (knownType)
            {
                switch (resultType)
                {
                    case TypeCode.Byte:
                    case TypeCode.SByte:
                    case TypeCode.UInt16:
                    case TypeCode.UInt32:
                    case TypeCode.UInt64:
                    case TypeCode.Int16:
                    case TypeCode.Int32:
                    case TypeCode.Int64:
                    case TypeCode.Decimal:
                    case TypeCode.Double:
                    case TypeCode.Single:
                        return true;
                    default:
                        return false;
                }
            }
            return false;
        }
    }
}
