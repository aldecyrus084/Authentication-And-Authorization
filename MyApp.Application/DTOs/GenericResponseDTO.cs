using System;
using System.Collections.Generic;
using System.Text;

namespace MyApp.Application.DTOs
{
    public class GenericResponseDTO<T>
    {
        public string message { get; set; }
        public bool isSuccess { get; set; }
        public T? Data { get; set; }
    }
}
