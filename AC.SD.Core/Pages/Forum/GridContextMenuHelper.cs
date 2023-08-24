using DevExpress.Blazor;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AC.SD.Core.Pages.Forum
{
    public enum GridContextMenuItemType
    {
        FullExpand, FullCollapse,
        SortAscending, SortDescending, ClearSorting,
        GroupByColumn, UngroupColumn, ClearGrouping, ShowGroupPanel,
        HideColumn, ShowColumnChooser,
        ClearFilter,
        ShowFilterRow, ShowFooter,

        ExpandRow, CollapseRow,
        ExpandDetailRow, CollapseDetailRow,
        NewRow, EditRow, DeleteRow,

        FixColumnToRight, FixColumnToLeft, Unfix,

        SaveUpdates, CancelUpdates
    }
    public class ContextMenuItem
    {
        public GridContextMenuItemType ItemType { get; set; }
        public string Text { get; set; }
        public bool Enabled { get; set; }
        public bool Visible { get; set; }
        public bool BeginGroup { get; set; }
        public string CssClass { get; set; }
        public string IconCssClass { get; set; }
    }
    public class GridContextMenuHelper
    {
       

      
       
            static List<ContextMenuItem> CreateRowContextMenuItems()
            {
                return new List<ContextMenuItem>
                {
                new ContextMenuItem { ItemType = GridContextMenuItemType.SaveUpdates, Text = "Save", IconCssClass="grid-context-menu-item-edit-row" },
                new ContextMenuItem { ItemType = GridContextMenuItemType.CancelUpdates, Text = "Cancel", IconCssClass="grid-context-menu-item-delete-row" },
                new ContextMenuItem { ItemType = GridContextMenuItemType.ExpandRow, Text = "Expand", IconCssClass="grid-context-menu-item-expand-row" },
                new ContextMenuItem { ItemType = GridContextMenuItemType.CollapseRow, Text = "Collapse", IconCssClass="grid-context-menu-item-collapse-row" },
                new ContextMenuItem { ItemType = GridContextMenuItemType.ExpandDetailRow, Text = "Expand Detail", BeginGroup = true, IconCssClass="grid-context-menu-item-expand-detail-row" },
                new ContextMenuItem { ItemType = GridContextMenuItemType.CollapseDetailRow, Text = "Collapse Detail", IconCssClass="grid-context-menu-item-collapse-detail-row" },
                new ContextMenuItem { ItemType = GridContextMenuItemType.NewRow, Text = "New", BeginGroup = true, IconCssClass="grid-context-menu-item-new-row" },
                new ContextMenuItem { ItemType = GridContextMenuItemType.EditRow, Text = "Edit", IconCssClass="grid-context-menu-item-edit-row" },
                new ContextMenuItem { ItemType = GridContextMenuItemType.DeleteRow, Text = "Delete", IconCssClass="grid-context-menu-item-delete-row" }
                };
            }
        
        public static bool IsContextMenuElement(GridElementType elementType)
        {
            return IsRowContextMenuElement(elementType);
        }
        public static async Task ProcessRowMenuItemClickAsync(ContextMenuItem item, int visibleIndex, IGrid grid)
        {
            switch (item.ItemType)
            {
                case GridContextMenuItemType.ExpandRow:
                    grid.ExpandGroupRow(visibleIndex);
                    break;
                case GridContextMenuItemType.CollapseRow:
                    grid.CollapseGroupRow(visibleIndex);
                    break;
                case GridContextMenuItemType.ExpandDetailRow:
                    grid.ExpandDetailRow(visibleIndex);
                    break;
                case GridContextMenuItemType.CollapseDetailRow:
                    grid.CollapseDetailRow(visibleIndex);
                    break;
                case GridContextMenuItemType.NewRow:
                    await grid.StartEditNewRowAsync();
                    break;
                case GridContextMenuItemType.EditRow:
                    await grid.StartEditRowAsync(visibleIndex);
                    break;
                case GridContextMenuItemType.DeleteRow:
                    grid.ShowRowDeleteConfirmation(visibleIndex);
                    break;
                case GridContextMenuItemType.SaveUpdates:
                    await grid.SaveChangesAsync();
                    break;
                case GridContextMenuItemType.CancelUpdates:
                    await grid.CancelEditAsync();
                    break;
            }
        }
        public static bool IsRowContextMenuElement(GridElementType elementType)
        {
            switch (elementType)
            {
                case GridElementType.DataRow:
                case GridElementType.GroupRow:
                case GridElementType.EditRow:
                    return true;
            }
            return false;
        }

        public static List<ContextMenuItem> GetRowItems(GridCustomizeElementEventArgs e)
        {
            var items = CreateRowContextMenuItems();
            foreach (var item in items)
            {
                item.Visible = IsRowMenuItemVisible(e, item.ItemType);
                item.Enabled = IsRowMenuItemEnabled(e, item.ItemType);
            }
            return items;
        }

        static bool IsRowMenuItemVisible(GridCustomizeElementEventArgs e, GridContextMenuItemType itemType)
        {
            var isGroupRow = e.Grid.IsGroupRow(e.VisibleIndex);
            var hasDetailButton = !isGroupRow
                        && e.Grid.DetailRowTemplate != null
                        && e.Grid.DetailRowDisplayMode == GridDetailRowDisplayMode.Auto
                        && e.Grid.DetailExpandButtonDisplayMode == GridDetailExpandButtonDisplayMode.Auto;
            switch (itemType)
            {
                case GridContextMenuItemType.ExpandRow:
                case GridContextMenuItemType.CollapseRow:
                    return isGroupRow;
                case GridContextMenuItemType.ExpandDetailRow:
                case GridContextMenuItemType.CollapseDetailRow:
                    return hasDetailButton;
                case GridContextMenuItemType.NewRow:
                    return true;
                case GridContextMenuItemType.EditRow:
                case GridContextMenuItemType.DeleteRow:
                    return !isGroupRow;
                case GridContextMenuItemType.SaveUpdates:
                case GridContextMenuItemType.CancelUpdates:
                    return e.Grid.IsEditing();
            }
            return false;
        }
      

        static bool IsRowMenuItemEnabled(GridCustomizeElementEventArgs e, GridContextMenuItemType itemType)
        {
            var isGroupRow = e.Grid.IsGroupRow(e.VisibleIndex);
            var isGroupRowExpanded = e.Grid.IsGroupRowExpanded(e.VisibleIndex);
            var hasDetailButton = !isGroupRow
                        && e.Grid.DetailRowTemplate != null
                        && e.Grid.DetailRowDisplayMode == GridDetailRowDisplayMode.Auto
                        && e.Grid.DetailExpandButtonDisplayMode == GridDetailExpandButtonDisplayMode.Auto;
            var isDetailRowExpanded = e.Grid.IsDetailRowExpanded(e.VisibleIndex);
            switch (itemType)
            {
                case GridContextMenuItemType.ExpandRow:
                    return isGroupRow && !isGroupRowExpanded;
                case GridContextMenuItemType.CollapseRow:
                    return isGroupRow && isGroupRowExpanded;
                case GridContextMenuItemType.ExpandDetailRow:
                    return hasDetailButton && !isDetailRowExpanded;
                case GridContextMenuItemType.CollapseDetailRow:
                    return hasDetailButton && isDetailRowExpanded;
                case GridContextMenuItemType.NewRow:
                case GridContextMenuItemType.EditRow:
                case GridContextMenuItemType.DeleteRow:
                    return !e.Grid.IsEditing();
                case GridContextMenuItemType.SaveUpdates:
                case GridContextMenuItemType.CancelUpdates:
                    return e.Grid.IsEditing();
            }
            return false;
        }
    }

}

