using System;
using System.Collections.Generic;
using System.Linq;

using Android.Content;
using Android.Text;
using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;

using Com.Bumptech.Glide;
using Com.Bumptech.Glide.Load.Engine;

using CNHSpotlight.WordPress.Models;

namespace CNHSpotlight.Components
{
    public class NewsRecyclerAdapter : RecyclerView.Adapter
    {

        // properties and fields
        Context context;

        public RecyclerView RecyclerView { get; private set; }

        public List<Post> PostList { get; private set; }

        public bool IsLoadingShown { get; private set; }

        // events
        public event EventHandler<ItemClickEventArgs> ItemClick;

        // constructor
        public NewsRecyclerAdapter(Context context, RecyclerView recyclerView, List<Post> posts)
        {
            this.context = context;
            RecyclerView = recyclerView;

            PostList = posts;
        }

        // another constructor
        public NewsRecyclerAdapter(Context context, RecyclerView recyclerView)
        {
            this.context = context;
            RecyclerView = recyclerView;

            PostList = new List<Post>();
        }

        public void AddItems(List<Post> newItem)
        {
            SetLoadingAnimation(false);

            int lastItemPos = PostList.Count - 1;

            PostList.AddRange(newItem);

            NotifyItemRangeInserted(lastItemPos + 1, newItem.Count);
        }

        public void ClearItems()
        {
            int lastPostListCount = PostList.Count;
            PostList.Clear();

            NotifyItemRangeRemoved(0, lastPostListCount);
        }

        public void ReplaceItems(List<Post> newItems)
        {
            ClearItems();

            PostList.AddRange(newItems);

            NotifyItemRangeInserted(0, newItems.Count);
        }

        /// <summary>
        /// Automatically called with 'false' before any items are added to the list.
        /// <para>
        /// But it is recommended to do that manually
        /// </para>
        /// </summary>
        /// <param name="state"></param>
        public void SetLoadingAnimation(bool state)
        {
            // turn on
            if (state && !IsLoadingShown)
            {
                PostList.Add(new DummyLoadingPost());

                NotifyItemInserted(PostList.Count - 1);

                IsLoadingShown = true;
            }
            else
            {
                if (IsLoadingShown)
                {
                    PostList.RemoveAt(PostList.Count - 1);

                    NotifyItemRemoved(PostList.Count);

                    IsLoadingShown = false;
                }
            }
        }

        public Post GetItem(int position)
        {
            if (position < PostList.Count)
            {
                return PostList[position];
            }
            else
            {
                return null;
            }
        }

        #region Abstract base implementation
        public override int ItemCount
        {
            get
            {
                return PostList.Count;
            }
        }

        public override int GetItemViewType(int position)
        {
            Post post = GetItem(position);

            if (post.GetType() == typeof(Post))
            {
                return (int)ViewType.Post;
            }
            if (post.GetType() == typeof(DummyLoadingPost))
            {
                return (int)ViewType.DummyLoadingPost;
            }

            return 0;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            switch ((ViewType)viewType)
            {
                default:
                case ViewType.Post:
                    View newsItemView =
                        LayoutInflater.FromContext(context).Inflate(Resource.Layout.PostCardViewItem, parent, false);

                    RecyclerViewHolder newHolder = new RecyclerViewHolder(newsItemView);
                    newHolder.ItemClick += OnItemClick;

                    return newHolder;
                case ViewType.DummyLoadingPost:
                    View loadingItemView =
                        LayoutInflater.FromContext(context).Inflate(Resource.Layout.ProgressbarViewItem, parent, false);

                    DummyLoadingViewHolder newLoadingHolder = new DummyLoadingViewHolder(loadingItemView);

                    return newLoadingHolder;
            }

        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {

            if (holder.GetType() == typeof(DummyLoadingViewHolder))
            {
                return;
            }

            Post currentPost = GetItem(position);

            RecyclerViewHolder currentViewHolder = (RecyclerViewHolder)holder;

            // update data
            #pragma warning disable CS0618
            currentViewHolder.Title.TextFormatted = Html.FromHtml(currentPost.Title.Rendered);

            // author
            MediaAuthor author = currentPost.Embedded.Author.FirstOrDefault();

            currentViewHolder.Author.Text = author != null ? author.Name : "Unknown";

            Glide.With(context)
                .Load(currentPost.Embedded.WpFeaturedMedia.FirstOrDefault().SourceUrl)
                .DontTransform()
                .DiskCacheStrategy(DiskCacheStrategy.All)
                .Placeholder(Resource.Drawable.placeholder)
                .Error(Resource.Drawable.placeholder_error)
                .Into(currentViewHolder.ThumbnailImage);
        } 
        #endregion

        void OnItemClick(int position)
        {
            ItemClick?.Invoke(this, new ItemClickEventArgs(position));
        }

    }

    class RecyclerViewHolder : RecyclerView.ViewHolder
    {
        public CardView Cardview { get; private set; }

        public TextView Title { get; private set; }

        public TextView Author { get; private set; }

        public ImageView ThumbnailImage { get; private set; }

        public event Action<int> ItemClick;

        public RecyclerViewHolder(View view) : base(view)
        {
            Cardview = (CardView)view;

            Title = view.FindViewById<TextView>(Resource.Id.postcardviewitem_text_title);
            ThumbnailImage = view.FindViewById<ImageView>(Resource.Id.postcardviewitem_imageview_thumbnailimage);
            Author = view.FindViewById<TextView>(Resource.Id.postcardviewitem_text_author);


            // hook click event
            Cardview.Click += (o, e) => { ItemClick?.Invoke(AdapterPosition); };
        }
    }

    class DummyLoadingViewHolder : RecyclerView.ViewHolder
    {
        public DummyLoadingViewHolder(View view) : base(view)
        {
        }
    }

    /// <summary>
    /// A dummy class which denotes a loading view
    /// </summary>
    class DummyLoadingPost : Post
    {
    }

    /// <summary>
    /// viewType constants -- mapped into enums
    /// </summary>
    enum ViewType
    {
        Post = 1,
        DummyLoadingPost = -1
    }

    public class ItemClickEventArgs : EventArgs
    {
        public int ItemPosition { get; private set; }
        public ItemClickEventArgs(int itemPos)
        {
            ItemPosition = itemPos;
        }
    }
}