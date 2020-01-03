using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Common.Constants
{
    /*
     *  Queue Naming Rules
     * 
     *  1. A queue name must start with a letter or number, and can only contain letters, numbers, and the dash (-) character.
     *  2. The first and last letters in the queue name must be alphanumeric. The dash (-) character cannot be the first or last character. Consecutive dash characters are not permitted in the queue name.
     *  3. All letters in a queue name must be lowercase.
     *  4. A queue name must be from 3 through 63 characters long.
     *  
     */

    public static class QueueNames
    {
        public readonly static string MainRecordTask = "content-views-update-main-record";
        public readonly static string DataLakeTask = "content-views-update-data-lake";
        public readonly static string CountTableTask = "content-views-update-count-table";
        public readonly static string ReportsTableTask = "content-views-update-report-table";
    }
}
