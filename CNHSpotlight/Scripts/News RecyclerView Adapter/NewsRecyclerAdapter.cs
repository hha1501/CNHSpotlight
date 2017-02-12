using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Android.Content;
using Android.Text;
using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;

using Com.Bumptech.Glide;
using Com.Bumptech.Glide.Load.Engine;

using CNHSpotlight.WordPress.Models;
using CNHSpotlight.WordPress;
using CNHSpotlight.Scripts.ConnectionInfo;
using CNHSpotlight.Asset;

namespace CNHSpotlight.Components
{
    public class NewsRecyclerAdapter : RecyclerView.Adapter
    {

        // properties and fields
        Context context;

        public List<Post> PostList { get; private set; }

        public bool IsLoading { get; private set; }

        public bool IsLoadingMore { get; private set; }

        public bool CanLoadMore { get; private set; }

        // data properties
        public CNHCategory Category { get; private set; }

        public string SearchKeyword { get; private set; }

        public bool IsSearchOn { get; private set; }

        // events
        public event EventHandler<ItemClickEventArgs> ItemClick;

        public event EventHandler Loading;
        public event EventHandler Loaded;

        public event EventHandler Error;
        public event EventHandler ConnectionError;
        public event EventHandler NoData;

        // events wrapper methods
        protected void OnLoading()
        {
            Loading?.Invoke(this, EventArgs.Empty);
        }
        protected void OnLoaded()
        {
            Loaded?.Invoke(this, EventArgs.Empty);
        }
        protected void OnError()
        {
            Error?.Invoke(this, EventArgs.Empty);
        }
        protected void OnConnectionError()
        {
            ConnectionError?.Invoke(this, EventArgs.Empty);
        }
        protected void OnNoData()
        {
            NoData?.Invoke(this, EventArgs.Empty);
        }

        // constructor
        public NewsRecyclerAdapter(Context context)
        {
            this.context = context;

            PostList = new List<Post>();

            Category = CNHCategory.Latest;

            SearchKeyword = "";

            CanLoadMore = true;
        }

        public async Task FetchNews(CNHCategory category)
        {
            // clear posts in case category is changed
            if (category != Category)
            {
                ClearItems();
            }

            IsLoading = true;
            OnLoading();

            WordPressManager.PostRequest postRequest = new WordPressManager.PostRequest();
            postRequest
                .Embeded()
                .Category(category)
                .Quantity(10)
                .Search(SearchKeyword)
                .Save(!IsSearchOn)
                .Update(IsSearchOn && ConnectionInfo.InternetConnected());
            var postsData = await WordpressExtension.GetPosts(postRequest);

            switch (postsData.Result)
            {
                case TaskResult.Error:
                    OnError();
                    break;
                case TaskResult.NoInternet:
                    OnConnectionError();
                    break;
                case TaskResult.NoData:
                    OnNoData();
                    break;
                case TaskResult.Success:
                    ReplaceItems(postsData.Data);
                    break;
                default:
                    break;
            }

            Category = category;

            IsLoading = false;
            OnLoaded();

            CanLoadMore = true;
        }

        public async Task RefreshNews()
        {
            IsLoading = true;
            OnLoading();

            WordPressManager.PostRequest postRequest = new WordPressManager.PostRequest();
            postRequest
                .Embeded()
                .Category(Category)
                .Quantity(10)
                .Search(SearchKeyword)
                .Update(true)
                .Save(!IsSearchOn);

            var postsData = await WordpressExtension.GetPosts(postRequest);

            switch (postsData.Result)
            {
                case TaskResult.Error:
                    OnError();
                    break;
                case TaskResult.NoInternet:
                    OnConnectionError();
                    break;
                case TaskResult.NoData:
                    OnNoData();
                    break;
                case TaskResult.Success:
                    ReplaceItems(postsData.Data);
                    break;
                default:
                    break;
            }

            IsLoading = false;
            OnLoaded();

            CanLoadMore = true;
        }

        public async Task LoadMore()
        {
            if (IsLoadingMore || !CanLoadMore)
            {
                return;
            }

            SetLoadingAnimation(true);

            WordPressManager.PostRequest postRequest = new WordPressManager.PostRequest();
            int indexSubstitution = IsLoadingMore ? 1 : 0;
            postRequest
                .Embeded()
                .Category(Category)
                .Quantity(10)
                .Offset(ItemCount - indexSubstitution)
                .Search(SearchKeyword)
                .Save(!IsSearchOn)
                .Update(IsSearchOn && ConnectionInfo.InternetConnected());

            var postsData = await WordpressExtension.GetPosts(postRequest);

            switch (postsData.Result)
            {
                case TaskResult.Error:
                    OnError();
                    break;
                case TaskResult.NoInternet:
                    OnConnectionError();
                    break;
                case TaskResult.NoData:
                    CanLoadMore = false;
                    break;
                case TaskResult.Success:
                    AddItems(postsData.Data);
                    break;
                default:
                    break;
            }

            SetLoadingAnimation(false);
        }

        public async Task Search(string keyword)
        {
            SearchKeyword = keyword;

            IsSearchOn = SearchKeyword != string.Empty;

            await FetchNews(Category);
        }

        #region Items manipulation
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

        public Post GetPost(int position)
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
        #endregion

        /// <summary>
        /// Automatically called with 'false' before any items are added to the list.
        /// <para>
        /// But it is recommended to do that manually
        /// </para>
        /// </summary>
        /// <param name="state"></param>
        void SetLoadingAnimation(bool state)
        {
            // turn on
            if (state && !IsLoadingMore)
            {
                PostList.Add(new DummyLoadingPost());

                NotifyItemInserted(PostList.Count - 1);

                IsLoadingMore = true;
            }
            else
            {
                if (IsLoadingMore)
                {
                    PostList.RemoveAt(PostList.Count - 1);

                    NotifyItemRemoved(PostList.Count);

                    IsLoadingMore = false;
                }
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
            Post post = GetPost(position);

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
                    newHolder.ItemClick += (o, e) => OnItemClick(e);

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

            Post currentPost = GetPost(position);

            RecyclerViewHolder currentViewHolder = (RecyclerViewHolder)holder;

            // update data
            #pragma warning disable CS0618
            currentViewHolder.Title.TextFormatted = Html.FromHtml(currentPost.Title.Rendered);

            // author
            MediaAuthor author = currentPost.Embedded.Author.FirstOrDefault();

            currentViewHolder.Author.Text = author != null ? author.Name : "Unknown";

            Glide.With(context)
                .Load(currentPost.Embedded.WpFeaturedMedia.FirstOrDefault().SourceUrl)
                .CenterCrop()
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

        public event EventHandler<int> ItemClick;

        public RecyclerViewHolder(View view) : base(view)
        {
            Cardview = (CardView)view;

            Title = view.FindViewById<TextView>(Resource.Id.postcardviewitem_text_title);
            Title.Typeface = AssetUtils.CustomFont;

            ThumbnailImage = view.FindViewById<ImageView>(Resource.Id.postcardviewitem_imageview_thumbnailimage);
            Author = view.FindViewById<TextView>(Resource.Id.postcardviewitem_text_author);


            // hook click event
            Cardview.Click += (o, e) => { ItemClick?.Invoke(this, AdapterPosition); };
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