using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PensionContributionManagementSystem.Core.Dtos.Request
{
    public class PaginationRequestDto
    {
        public int pageSize { get; set; }
        public int offset { get; set; }
    }
}
