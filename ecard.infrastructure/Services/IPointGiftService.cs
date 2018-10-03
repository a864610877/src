using System;
using System.Collections.Generic;
using Ecard.Models;

namespace Ecard.Services
{
    public interface IPointGiftService
    {
        IEnumerable<PointGift> Query();
        void Create(PointGift item);
        PointGift GetById(int id);
        void Update(PointGift item);
        void Delete(PointGift item);
    }
}