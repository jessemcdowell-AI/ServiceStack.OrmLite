using System;
using System.Collections.Generic;
using System.Data;
using NUnit.Framework;
using ServiceStack.Common.Tests.Models;
using ServiceStack.Text;
using ServiceStack.OrmLite;

namespace ServiceStack.OrmLite.Tests
{
    [TestFixture]
    public class OrmLiteUpdateTests
        : OrmLiteTestBase
    {
        private IDbConnection db;

        [SetUp]
        public void SetUp()
        {
            db = OpenDbConnection();
        }

        [TearDown]
        public void TearDown()
        {
            db.Dispose();
        }

        private ModelWithFieldsOfDifferentTypes CreateModelWithFieldsOfDifferentTypes()
        {
            db.DropAndCreateTable<ModelWithFieldsOfDifferentTypes>();

            var row = ModelWithFieldsOfDifferentTypes.Create(1);
            return row;
        }

        [Test]
        public void Can_update_ModelWithFieldsOfDifferentTypes_table()
        {
            var row = CreateModelWithFieldsOfDifferentTypes();

            row.Id = (int)db.Insert(row, selectIdentity: true);

            row.Name = "UpdatedName";

            db.Update(row);

            var dbRow = db.SingleById<ModelWithFieldsOfDifferentTypes>(row.Id);

            ModelWithFieldsOfDifferentTypes.AssertIsEqual(dbRow, row);
        }

        [Test]
        public void Can_update_ModelWithFieldsOfDifferentTypes_table_with_filter()
        {
            var row = CreateModelWithFieldsOfDifferentTypes();

            row.Id = (int)db.Insert(row, selectIdentity: true);

            row.Name = "UpdatedName";

            db.Update(row, x => x.LongId <= row.LongId);

            var dbRow = db.SingleById<ModelWithFieldsOfDifferentTypes>(row.Id);

            ModelWithFieldsOfDifferentTypes.AssertIsEqual(dbRow, row);
        }

        [Test]
        public void Can_update_with_anonymousType_and_expr_filter()
        {
            var row = CreateModelWithFieldsOfDifferentTypes();

            row.Id = (int)db.Insert(row, selectIdentity: true);
            row.DateTime = DateTime.Now;
            row.Name = "UpdatedName";

            db.Update<ModelWithFieldsOfDifferentTypes>(new { row.Name, row.DateTime },
                x => x.LongId >= row.LongId && x.LongId <= row.LongId);

            var dbRow = db.SingleById<ModelWithFieldsOfDifferentTypes>(row.Id);
            Console.WriteLine(dbRow.Dump());
            ModelWithFieldsOfDifferentTypes.AssertIsEqual(dbRow, row);
        }

        [Test]
        public void Can_update_with_optional_string_params()
        {
            var row = CreateModelWithFieldsOfDifferentTypes();

            row.Id = (int)db.Insert(row, selectIdentity: true);
            row.Name = "UpdatedName";

            db.UpdateFmt<ModelWithFieldsOfDifferentTypes>(set: "NAME = {0}".SqlFmt(row.Name), where: "LongId".SqlColumn() + " <= {0}".SqlFmt(row.LongId));

            var dbRow = db.SingleById<ModelWithFieldsOfDifferentTypes>(row.Id);
            Console.WriteLine(dbRow.Dump());
            ModelWithFieldsOfDifferentTypes.AssertIsEqual(dbRow, row);
        }

        [Test]
        public void Can_update_with_tableName_and_optional_string_params()
        {
            var row = CreateModelWithFieldsOfDifferentTypes();

            row.Id = (int)db.Insert(row, selectIdentity: true);
            row.Name = "UpdatedName";

            db.UpdateFmt(table: "ModelWithFieldsOfDifferentTypes".SqlTableRaw(),
                set: "NAME = {0}".SqlFmt(row.Name), where: "LongId".SqlColumn() + " <= {0}".SqlFmt(row.LongId));

            var dbRow = db.SingleById<ModelWithFieldsOfDifferentTypes>(row.Id);
            Console.WriteLine(dbRow.Dump());
            ModelWithFieldsOfDifferentTypes.AssertIsEqual(dbRow, row);
        }

        [Test]
        public void Can_Update_Into_Table_With_Id_Only()
        {
            db.CreateTable<ModelWithIdOnly>(true);
            var row1 = new ModelWithIdOnly(1);
            db.Insert(row1);

            db.Update(row1);
        }

        [Test]
        public void Can_Update_Many_Into_Table_With_Id_Only()
        {
            db.CreateTable<ModelWithIdOnly>(true);
            var row1 = new ModelWithIdOnly(1);
            var row2 = new ModelWithIdOnly(2);
            db.Insert(row1, row2);

            db.Update(row1, row2);

            var list = new List<ModelWithIdOnly> { row1, row2 };
            db.UpdateAll(list);
        }

        [Test]
        public void Can_Update_Into_Table_With_RowVersion()
        {
            db.CreateTable<ModelWithRowVersionField>(true);

            var row1 = new ModelWithRowVersionField(1);
            db.Insert(row1);

            var readRow = db.SingleById<ModelWithRowVersionField>(1);

            readRow.ChangeableField = "ChangedValue";
            db.Update(readRow);
        }

        [Test]
        public void Get_Exception_On_Second_Update_Into_Table_With_RowVersion()
        {
            db.CreateTable<ModelWithRowVersionField>(true);
            
            var row1 = new ModelWithRowVersionField(1);
            db.Insert(row1);

            var readRow = db.SingleById<ModelWithRowVersionField>(1);

            readRow.ChangeableField = "ChangedValue";
            db.Update(readRow);

            readRow.ChangeableField = "ValueChangedWithoutRereading";
            Assert.Throws<RowModifiedException>(() => db.Update(readRow));
        }

        [Test]
        public void Perform_Second_Update_Into_Table_Without_RowVersion()
        {
            db.DropAndCreateTable<ModelWithIdAndName>();

            var row1 = new ModelWithIdAndName(1);
            db.Insert(row1);

            var readRow = db.SingleById<ModelWithIdAndName>(row1.Id);

            readRow.Name = "ChangedName";
            db.Update(readRow);

            readRow.Name = "NameChangedWithoutRereading";
            db.Update(readRow);
        }

    }

}
