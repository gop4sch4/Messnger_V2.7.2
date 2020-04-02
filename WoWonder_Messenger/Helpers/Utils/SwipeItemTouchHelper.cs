using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Support.V7.Widget;
using Android.Support.V7.Widget.Helper;
using Android.Views;
using System;
using WoWonder.Activities.OldTab.Adapter;
using WoWonder.Activities.Tab.Adapter;
using WoWonder.Helpers.Model;

namespace WoWonder.Helpers.Utils
{
    public class SwipeItemTouchHelper : ItemTouchHelper.Callback
    {
        private static readonly float AlphaFull = 1.0f;
        private Color BgColorCode = Color.Transparent;

        private readonly LastMessagesAdapter MAdapter;
        private readonly LastChatsAdapter MNewAdapter;

        public SwipeItemTouchHelper(LastMessagesAdapter adapter)
        {
            try
            {
                MAdapter = adapter;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public SwipeItemTouchHelper(LastChatsAdapter adapter)
        {
            try
            {
                MNewAdapter = adapter;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public override bool IsLongPressDragEnabled => false;
        public override bool IsItemViewSwipeEnabled => true;

        public override int GetMovementFlags(RecyclerView recyclerView, RecyclerView.ViewHolder viewHolder)
        {
            try
            {
                // Set movement flags based on the layout manager
                if (recyclerView.GetLayoutManager() is GridLayoutManager)
                {
                    var dragFlags = ItemTouchHelper.Up | ItemTouchHelper.Down | ItemTouchHelper.Left | ItemTouchHelper.Right;
                    var swipeFlags = 0;
                    return MakeMovementFlags(dragFlags, swipeFlags);
                }
                else
                {
                    var dragFlags = ItemTouchHelper.Up | ItemTouchHelper.Down;
                    var swipeFlags = ItemTouchHelper.Start | ItemTouchHelper.End;
                    return MakeMovementFlags(dragFlags, swipeFlags);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return 0;
            }
        }

        public override bool OnMove(RecyclerView recyclerView, RecyclerView.ViewHolder viewHolder, RecyclerView.ViewHolder target)
        {
            try
            {
                if (viewHolder.ItemViewType != target.ItemViewType) return false;
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return true;
            }
        }

        public override void OnSwiped(RecyclerView.ViewHolder viewHolder, int direction)
        {
            try
            {
                // Notify the adapter of the dismissal
                if (AppSettings.LastChatSystem == SystemApiGetLastChat.New)
                {
                    MNewAdapter.OnItemDismiss(viewHolder.AdapterPosition);
                }
                else
                {
                    MAdapter.OnItemDismiss(viewHolder.AdapterPosition);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public override void OnChildDraw(Canvas c, RecyclerView recyclerView, RecyclerView.ViewHolder viewHolder, float dX, float dY, int actionState, bool isCurrentlyActive)
        {
            try
            {
                if (actionState == ItemTouchHelper.ActionStateSwipe)
                {
                    View itemView = viewHolder.ItemView;
                    Drawable background = new ColorDrawable();
                    ((ColorDrawable)background).Color = GetBgColorCode();

                    if (dX > 0)
                    {
                        // swipe right
                        background.SetBounds(itemView.Left, itemView.Top, (int)dX, itemView.Bottom);
                    }
                    else
                    { // swipe left
                        background.SetBounds(itemView.Right + (int)dX, itemView.Top, itemView.Right, itemView.Bottom);
                    }
                    background.Draw(c);
                }

                base.OnChildDraw(c, recyclerView, viewHolder, dX, dY, actionState, isCurrentlyActive);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public override void OnSelectedChanged(RecyclerView.ViewHolder viewHolder, int actionState)
        {
            try
            {
                // We only want the active item to change
                if (actionState != ItemTouchHelper.ActionStateIdle)
                {
                    if (viewHolder is ITouchViewHolder itemViewHolder)
                    {
                        // Let the view holder know that this item is being moved or dragged

                        itemViewHolder.OnItemSelected();
                    }
                }

                base.OnSelectedChanged(viewHolder, actionState);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public override void ClearView(RecyclerView recyclerView, RecyclerView.ViewHolder viewHolder)
        {
            try
            {
                base.ClearView(recyclerView, viewHolder);

                viewHolder.ItemView.Alpha = AlphaFull;

                if (viewHolder is ITouchViewHolder itemViewHolder)
                {
                    // Tell the view holder it's time to restore the idle state
                    itemViewHolder.OnItemClear();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public Color GetBgColorCode()
        {
            return BgColorCode;
        }

        public void SetBgColorCode(Color bgColorCode)
        {
            BgColorCode = bgColorCode;
        }

        public interface ISwipeHelperAdapter
        {
            void OnItemDismiss(int position);
        }

        public interface ITouchViewHolder
        {
            void OnItemSelected();

            void OnItemClear();
        }
    }
}