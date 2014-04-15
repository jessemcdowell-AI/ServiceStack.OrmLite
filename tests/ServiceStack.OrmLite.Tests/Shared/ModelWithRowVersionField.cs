using ServiceStack.DataAnnotations;

namespace ServiceStack.Common.Tests.Models
{
    class ModelWithRowVersionField
    {
        public ModelWithRowVersionField(long id)
        {
            Id = id;
            RowVersion = 1;
            ChangeableField = "Changeable" + id;
        }        
        
        public long Id { get; set; }

        [RowVersion]
        [Default(0)]
        public long RowVersion { get; set; }

        public string ChangeableField { get; set; }
    }
}
