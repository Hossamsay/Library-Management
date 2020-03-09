using LibraryData;
using LibraryData.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LibraryServices
{
    public class LibraryAssetService : ILibraryAsset
    {
        private LibraryContext _context;
        public LibraryAssetService (LibraryContext context)
        {
            _context = context;
        }
        public void Add(LibraryAsset newAsset)
        {
            _context.Add(newAsset);
            _context.SaveChanges();
        }

       

        public IEnumerable<LibraryAsset> GetAll()
        {
            return _context.LibraryAssets
                .Include(s => s.Status)
                .Include(s => s.Location);
        }
        public LibraryAsset GetById(int id)
        {
            return _context.LibraryAssets
                .Include(s => s.Status)
                .Include(s => s.Location)
                .FirstOrDefault(s => s.Id == id);
        }


        public LibraryBranch GetCurrentLocation(int id)
        {
            return _context.LibraryAssets.First(a => a.Id == id).Location;
        }

        public string GetDeweyIndex(int id)
        {
            if (_context.Books.Any(book => book.Id == id))
            {
                return _context.Books.FirstOrDefault(book => book.Id == id).DeweyIndex;
            }
            else return "";
        }

        public string GetIsbn(int id)
        {
            if (_context.Books.Any(book => book.Id == id))
            {
                return _context.Books.FirstOrDefault(book => book.Id == id).ISBN;
            }
            else return "";
        }

        public LibraryCard GetLibraryCardByAssetId(int id)
        {
            return _context.LibraryCards
                .FirstOrDefault(c => c.Checkouts
                    .Select(a => a.LibraryAsset)
                    .Select(v => v.Id).Contains(id));
        }

        public string GetTitle(int id)
        {
            if (_context.Books.Any(book => book.Id == id))
            {
                return _context.Books.FirstOrDefault(book => book.Id == id).Title;
            }
            else return "";
        }

        public string GetType(int id)
        {
            var book = _context.LibraryAssets.OfType<Book>().Where(a => a.Id == id);
            return book.Any() ? "Book" : "Video";
        }
       
        public string GetAuthorOrDirector(int id)
        {
            var isBook = _context.LibraryAssets.OfType<Book>().Where(a => a.Id == id).Any();
            var isVideo = _context.LibraryAssets.OfType<Video>().Where(a => a.Id == id).Any();

            return isBook ?
                _context.Books.FirstOrDefault(a => a.Id == id).Author :
                _context.Videos.FirstOrDefault(v => v.Id == id).Director ??
                "UnKnown";
        }
    }
}
