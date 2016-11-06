using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Android.Text;
using Android.Text.Style;

using Java.Lang;

using CNHSpotlight.ImageResource;


namespace CNHSpotlight
{
    class HtmlReader
    {
        /// <summary>
        /// Handle basic tags in html source
        /// <para>
        /// If images are required, use <see cref="GetReadableFromHtml(string, int)"/> overload
        /// </para>
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static ICharSequence GetReadableFromHtml(string source)
        {
            #pragma warning disable CS0618
            return Html.FromHtml(source);
        }

        /// <summary>
        /// Handles all images in source
        /// </summary>
        /// <param name="source"></param>
        /// <param name="postId">To perform an offline images lookup</param>
        /// <returns></returns>
        public static async Task<ICharSequence> GetReadableFromHtml(string source, int postId)
        {

            #pragma warning disable CS0618
            string renderedSource = Html.FromHtml(source).ToString();

            // at this point, images in source (noted as <img> tag) have been replaced with
            // Object replacement character \uFFFC
            SpannableString spanString = new SpannableString(renderedSource);


            // try to replace each Object replacement character with its image
            Regex regex = new Regex("\uFFFC");

            // a counter to get corresponding image from images
            int imageIndex = 0;

            foreach (Match match in regex.Matches(renderedSource))
            {

                // try to get image
                ImageSpan imageSpan = null;

                imageSpan = await  ImageGetter.GetPostImage(source, postId, imageIndex);


                // get the Object replacement character's bound
                int startPos = match.Index;
                int endPos = match.Index + match.Length;


                // Apply image to that bound
                spanString.SetSpan(imageSpan, startPos, endPos, SpanTypes.Composing);


                // increment matchIndex
                imageIndex++;
            }

            return spanString;
        }

    }

}