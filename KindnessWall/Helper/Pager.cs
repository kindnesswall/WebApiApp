using System;
using System.Collections.Generic;
using System.Linq;

namespace KindnessWall.Helper
{
    public class Pager
    {
        public Pager(int totalItems, int? page, int pageSize = 10)
        {
            var totalPages = (int)Math.Ceiling((decimal)totalItems / (decimal)pageSize);
            var currentPage = page ?? 1;
            var startPage = currentPage - 5;
            var endPage = currentPage + 4;
            if (startPage <= 0)
            {
                endPage -= (startPage - 1);
                startPage = 1;
            }
            if (endPage > totalPages)
            {
                endPage = totalPages;
                if (endPage > 10)
                {
                    startPage = endPage - 9;
                }
            }

            TotalItems = totalItems;
            CurrentPage = currentPage;
            PageSize = pageSize;
            TotalPages = totalPages;
            StartPage = startPage;
            EndPage = endPage;
        }

        public int TotalItems { get; private set; }
        public int CurrentPage { get; private set; }
        public int PageSize { get; private set; }
        public int TotalPages { get; private set; }
        public int StartPage { get; private set; }
        public int EndPage { get; private set; }
    }
    public class PagerViewModel<T>
    {
        public Pager Pager => new Pager(_items.Count(), Page);

        public int? Page { get; set; }

        private IEnumerable<T> _items { get; set; }

        public IEnumerable<T> Items
        {
            get { return _items.Skip((Pager.CurrentPage - 1) * Pager.PageSize).Take(Pager.PageSize).ToList(); }
            set { _items = value; }
        }

    }
}