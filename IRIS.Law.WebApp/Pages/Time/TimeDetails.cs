using System;

namespace IRIS.Law.WebApp.Pages.Time
{
    public class TimeDetails
    {
        private int _id;
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
        public int Id
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
            }
        }

        private Guid _projectId;
        /// <summary>
        /// Gets or sets the Project Id.
        /// </summary>
        /// <value>The Project Id.</value>
        public Guid ProjectId
        {
            get
            {
                return _projectId;
            }
            set
            {
                _projectId = value;
            }
        }

        private Guid _feeEarnerId;
        /// <summary>
        /// Gets or sets the Fee Earner Id.
        /// </summary>
        /// <value>The Fee Earner Id.</value>
        public Guid FeeEarnerId
        {
            get
            {
                return _feeEarnerId;
            }
            set
            {
                _feeEarnerId = value;
            }
        }

        private Guid _timeTypeId;
        /// <summary>
        /// Gets or sets the time type id.
        /// </summary>
        /// <value>The time type id.</value>
        public Guid TimeTypeId
        {
            get
            {
                return _timeTypeId;
            }
            set
            {
                _timeTypeId = value;
            }
        }

        private int _units;
        /// <summary>
        /// Gets or sets the Units.
        /// </summary>
        /// <value>The Units.</value>
        public int Units
        {
            get
            {
                return _units;
            }
            set
            {
                _units = value;
            }
        }

        private string _notes;
        /// <summary>
        /// Gets or sets the Notes.
        /// </summary>
        /// <value>The Notes.</value>
        public string Notes
        {
            get
            {
                return _notes;
            }
            set
            {
                _notes = value;
            }
        }


        private int _timeTypeCatId;
        /// <summary>
        /// Gets or sets the timeTypeCatId.
        /// </summary>
        /// <value>The Units.</value>
        public int TimeTypeCatId
        {
            get
            {
                return _timeTypeCatId;
            }
            set
            {
                _timeTypeCatId = value;
            }
        }


        private DateTime _date;
        /// <summary>
        /// Gets or sets the Date.
        /// </summary>
        /// <value>The Date.</value>
        public DateTime Date
        {
            get
            {
                return _date;
            }
            set
            {
                _date = value;
            }
        }


        private bool _canProceed;
        /// <summary>
        /// Gets or sets a value indicating whether this instance can proceed if limits are exceeded
        /// </summary>
        /// <value>Can Proceed.</value>
        public bool CanProceed
        {
            get
            {
                return _canProceed;
            }
            set
            {
                _canProceed = value;
            }
        }

    }
}