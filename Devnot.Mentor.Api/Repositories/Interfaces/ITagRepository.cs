﻿using DevnotMentor.Api.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevnotMentor.Api.Repositories.Interfaces
{
    public interface ITagRepository : IRepository<Tag>
    {
        Tag Get(string tagName);
    }
}
