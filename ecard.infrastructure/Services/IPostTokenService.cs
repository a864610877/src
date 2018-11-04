using Ecard.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ecard.Services
{
    public interface IPostTokenService
    {
        void Insert(PostToken item);
        void Update(PostToken item);
        PostToken GetByToken(string token);
        PostToken GetByPosName(string posName);
    }
}
