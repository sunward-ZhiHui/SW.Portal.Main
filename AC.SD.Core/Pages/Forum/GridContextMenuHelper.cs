using DevExpress.Blazor;
using System.Collections.Generic;
using System.Threading.Tasks;

using DevExpress.Blazor;
using System;

namespace GridWithContextMenu.Data
{


    public enum GridContextMenuItemType
    {
        ExpandRow, CollapseRow,
        ExpandDetailRow, CollapseDetailRow,
        NewRow, EditRow, DeleteRow,
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
            return new List<ContextMenuItem> {
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
    }

    //public static async Task ProcessRowMenuItemClickAsync(ContextMenuItem item, int visibleIndex, IGrid grid)
    //{
    //    switch (item.ItemType)
    //    {
    //        case GridContextMenuItemType.ExpandRow:
    //            grid.ExpandGroupRow(visibleIndex);
    //            break;
    //        case GridContextMenuItemType.CollapseRow:
    //            grid.CollapseGroupRow(visibleIndex);
    //            break;
    //        case GridContextMenuItemType.ExpandDetailRow:
    //            grid.ExpandDetailRow(visibleIndex);
    //            break;
    //        case GridContextMenuItemType.CollapseDetailRow:
    //            grid.CollapseDetailRow(visibleIndex);
    //            break;
    //        case GridContextMenuItemType.NewRow:
    //            await grid.StartEditNewRowAsync();
    //            break;
    //        case GridContextMenuItemType.EditRow:
    //            await grid.StartEditRowAsync(visibleIndex);
    //            break;
    //        case GridContextMenuItemType.DeleteRow:
    //            grid.ShowRowDeleteConfirmation(visibleIndex);
    //            break;
    //        case GridContextMenuItemType.SaveUpdates:
    //            await grid.SaveChangesAsync();
    //            break;
    //        case GridContextMenuItemType.CancelUpdates:
    //            await grid.CancelEditAsync();
    //            break;
    //    }
    //}
    //public static bool IsRowContextMenuElement(GridElementType elementType)
    //{
    //    switch (elementType)
    //    {
    //        case GridElementType.DataRow:
    //        case GridElementType.GroupRow:
    //        case GridElementType.EditRow:
    //            return true;
    //    }
    //    return false;
    //}

    //public static List<ContextMenuItem> GetRowItems(GridCustomizeElementEventArgs e)
    //{
    //    var items = CreateRowContextMenuItems();
    //    foreach (var item in items)
    //    {
    //        item.Visible = IsRowMenuItemVisible(e, item.ItemType);
    //        item.Enabled = IsRowMenuItemEnabled(e, item.ItemType);
    //    }
    //    return items;
    //}

    //IEnumerable<object> CreateRowContextMenuItems()
    //{
    //    throw new NotImplementedException();
    //}
}
