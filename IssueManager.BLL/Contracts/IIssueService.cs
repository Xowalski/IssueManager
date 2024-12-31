using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IssueManager.BLL.Contracts
{
    public interface IIssueService
    {
        Task AddNewIssueAsync(string owner, string repository, string title, string description);
        Task EditIssueAsync(string owner, string repository, int issueId, string newTitle, string newDescription);
        Task CloseIssueAsync(string owner, string repository, int issueId);
    }
}
