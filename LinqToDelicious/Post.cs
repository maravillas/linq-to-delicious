using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Globalization;
using System.Diagnostics;

namespace LinqToDelicious
{
    /// <summary>
    /// A post made to Delicious.
    /// </summary>
    public class Post
    {
        /// <summary>
        /// The address for this post.
        /// </summary>
        public String Address { get; private set; }

        /// <summary>
        /// The Delicious-generated hash for this post.
        /// </summary>
        public String Hash { get; private set; }

        /// <summary>
        /// The description for this post.
        /// </summary>
        public String Description { get; private set; }

        /// <summary>
        /// The list of tags for this post.
        /// </summary>
        public List<String> Tags { get; private set; }

        /// <summary>
        /// The extended text for this post.
        /// </summary>
        public String Extended { get; private set; }

        /// <summary>
        /// The date this post was saved.
        /// </summary>
        public DateTime Date { get; private set; }

        /// <summary>
        /// The signature that indicates when values have changed in this post.
        /// </summary>
        public String Meta { get; private set; }

        /// <summary>
        /// Create a new post.
        /// </summary>
        /// <param name="address">The address for this post.</param>
        /// <param name="hash">The Delicious-generated hash for this post.</param>
        /// <param name="description">The description for this post.</param>
        /// <param name="tags">The list of tags for this post.</param>
        /// <param name="extended">The extended text for this post.</param>
        /// <param name="date">The date this post was saved.</param>
        public Post(String address, String hash, String description, String tags, String extended, String date) :
            this(address, hash, description, tags, extended, date, "")
        {
            
        }

        /// <summary>
        /// Create a new post.
        /// </summary>
        /// <param name="address">The address for this post.</param>
        /// <param name="hash">The Delicious-generated hash for this post.</param>
        /// <param name="description">The description for this post.</param>
        /// <param name="tags">The list of tags for this post.</param>
        /// <param name="extended">The extended text for this post.</param>
        /// <param name="date">The date this post was saved.</param>
        /// <param name="meta">The signature that indicates when values have changed in this post.</param>
        public Post(String address, String hash, String description, String tags, String extended, String date, String meta)
        {
            Address = address;
            Hash = hash;
            Description = description;
            Tags = new List<String>(tags.Split(' '));
            Extended = extended;
            Date = DateTime.ParseExact(date, "yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'", new CultureInfo("en-US"));
            Meta = meta;
        }

        public override string ToString()
        {
            return String.Format("Post [address={0} hash={1} description={2} tags={3} extended={4} date={5} meta={5}]",
                Address, Hash, Description, Tags, Extended, Date, Meta);
        }

        public override bool Equals(object obj)
        {
            Post post = obj as Post;

            if ((System.Object)post == null)
            {
                return false;
            }

            if (post == this)
            {
                return true;
            }

            return Address.Equals(post.Address) &&
                Hash.Equals(post.Hash) &&
                Description.Equals(post.Description) &&
                Tags.SequenceEqual(post.Tags) &&
                Extended.Equals(post.Extended) &&
                Date.Equals(post.Date) &&
                Meta.Equals(post.Meta);
        }

        public override int GetHashCode()
        {
            return Address.GetHashCode() ^
                Hash.GetHashCode() ^
                Description.GetHashCode() ^
                string.Join(" ", Tags.ToArray()).GetHashCode() ^
                Extended.GetHashCode() ^
                Date.GetHashCode() ^
                Meta.GetHashCode();
        }
    }
}
