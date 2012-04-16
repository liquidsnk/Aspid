using System;
using System.Runtime.Serialization;

namespace Aspid.Core.Entities
{
    /// <summary>
    /// Represents information for paginated results.
    /// </summary>
    [Serializable]
    [DataContract]
    public class PaginationOutputInformation
    {
        public static PaginationOutputInformation Default
        {
            get
            {
                return new PaginationOutputInformation(0, 0, 0);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PaginationOutputInformation"/> class.
        /// </summary>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageAmount">The page amount.</param>
        /// <param name="count">The count.</param>
        public PaginationOutputInformation(int? pageNumber, int pageAmount, int count)
        {
            ResultsArePaginated = pageNumber.HasValue;
            TotalItemsCount = count;
            MaxItemsPerPage = pageAmount;
            PageCount = Math.Max(1, (int)Math.Ceiling(count / (pageAmount * 1.0)));
            CurrentPage = pageNumber ?? 1;
            FirstItem = (CurrentPage * MaxItemsPerPage) + 1;
            LastItem = Math.Min(TotalItemsCount, FirstItem + MaxItemsPerPage - 1);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the results are paginated.
        /// </summary>
        /// <value><c>true</c> if the results are paginated; otherwise, <c>false</c>.</value>
        [DataMember]
        public bool ResultsArePaginated { get; set; }

        /// <summary>
        /// Gets or sets the total items count.
        /// </summary>
        /// <value>The total items count.</value>
        [DataMember]
        public int TotalItemsCount { get; set; }

        /// <summary>
        /// Gets or sets the max items per page.
        /// </summary>
        /// <value>The max items per page.</value>
        [DataMember]
        public int MaxItemsPerPage { get; set; }

        /// <summary>
        /// Gets or sets the page count.
        /// </summary>
        /// <value>The page count.</value>
        [DataMember]
        public int PageCount { get; set; }

        /// <summary>
        /// Gets or sets the current page index.
        /// </summary>
        /// <value>The current page index.</value>
        [DataMember]
        public int CurrentPage { get; set; }

        /// <summary>
        /// Gets or sets the first item position number.
        /// </summary>
        /// <value>The first item number.</value>
        [DataMember]
        public int FirstItem { get; set; }

        /// <summary>
        /// Gets or sets the last item position number.
        /// </summary>
        /// <value>The last item position number.</value>
        [DataMember]
        public int LastItem { get; set; }
    }
}
