using Application.Response;
using Core.Entities.Base;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands
{
    public class CreateCommand<T> : IRequest<T> where T : class
    {      
        public T Entity { get; set; }     

    }
}
