using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Globalization;
using System.Diagnostics;

namespace LinqToDelicious
{
    public class Post
    {
        public String Address { get; private set; }
        public String Hash { get; private set; }
        public String Description { get; private set; }
        public List<String> Tags { get; private set; }
        public String Extended { get; private set; }
        public DateTime Date { get; private set; }
        public String Meta { get; private set; }

        public Post(String address, String hash, String description, String tags, String extended, String date) :
            this(address, hash, description, tags, extended, date, "")
        {
            
        }

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
