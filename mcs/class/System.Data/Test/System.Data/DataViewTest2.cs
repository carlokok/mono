// Authors:
//   Rafael Mizrahi   <rafim@mainsoft.com>
//   Erez Lotan       <erezl@mainsoft.com>
//   Oren Gurfinkel   <oreng@mainsoft.com>
//   Ofer Borstein
// 
// Copyright (c) 2004 Mainsoft Co.
// 
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using NUnit.Framework;
using System;
using System.IO;
using System.ComponentModel;
using System.Data;
using MonoTests.System.Data.Test.Utils;

namespace MonoTests_System.Data
{
	[TestFixture] public class DataViewTest2
	{
		private EventProperties evProp = null;

		class EventProperties  //hold the event properties to be checked
		{
			public System.ComponentModel.ListChangedType lstType ;
			public int NewIndex;
			public int OldIndex;
		}

		[Test] public void AddNew()
		{
			//create the source datatable
			DataTable dt = DataProvider.CreateChildDataTable();

			//create the dataview for the table
			DataView dv = new DataView(dt);

			int CountView = dv.Count ;
			int CountTable= dt.Rows.Count ;

			DataRowView drv = null;

			// AddNew - DataView Row Count
			drv = dv.AddNew();
			Assert.AreEqual(dv.Count , CountView+1, "DV1");

			// AddNew - Table Row Count 
			Assert.AreEqual(dt.Rows.Count , CountTable, "DV2");

			// AddNew - new row in DataTable
			drv.EndEdit();
			Assert.AreEqual(dt.Rows.Count , CountTable+1, "DV3");

			// AddNew - new row != null
			Assert.AreEqual(true, drv!=null, "DV4");

			// AddNew - check table
			Assert.AreEqual(dt, drv.Row.Table, "DV5");
		}

		[Test] public void AllowDelete()
		{
			DataTable dt = DataProvider.CreateParentDataTable();
			DataView dv = new DataView(dt);

			// AllowDelete - default value
			Assert.AreEqual(true , dv.AllowDelete , "DV6");

			// AllowDelete - true
			dv.AllowDelete = true;
			Assert.AreEqual(true, dv.AllowDelete , "DV7");

			// AllowDelete - false
			dv.AllowDelete = false;
			Assert.AreEqual(false, dv.AllowDelete , "DV8");

			dv.AllowDelete = false;
			// AllowDelete false- Exception
			try 
			{
				dv.Delete(0);
				Assert.Fail("DV9: Delete Failed to throw DataException");
			}
			catch (DataException) {}
			catch (AssertionException exc) {throw  exc;}
			catch (Exception exc)
			{
				Assert.Fail("DV10: Delete. Wrong exception type. Got:" + exc);
			}

			dv.AllowDelete = true;
			int RowsCount = dv.Count ;
			// AllowDelete true- Exception
			dv.Delete(0);
			Assert.AreEqual(RowsCount-1, dv.Count , "DV11");
		}

		[Test] public void AllowEdit()
		{
			DataTable dt = DataProvider.CreateParentDataTable();
			DataView dv = new DataView(dt);

			// AllowEdit - default value
			Assert.AreEqual(true , dv.AllowEdit , "DV12");

			// AllowEdit - true
			dv.AllowEdit = true;
			Assert.AreEqual(true, dv.AllowEdit , "DV13");

			// AllowEdit - false
			dv.AllowEdit = false;
			Assert.AreEqual(false, dv.AllowEdit , "DV14");

			dv.AllowEdit=false;

			// AllowEdit false - exception
			try 
			{
				dv[0][2] = "aaa";
				Assert.Fail("DV15: Indexer Failed to throw DataException");
			}
			catch (DataException) {}
			catch (AssertionException exc) {throw  exc;}
			catch (Exception exc)
			{
				Assert.Fail("DV16: Indexer. Wrong exception type. Got:" + exc);
			}

			dv.AllowEdit=true;

			// AllowEdit true- exception
			dv[0][2] = "aaa";
			Assert.AreEqual("aaa", dv[0][2] , "DV17");
		}

		[Test] public void AllowNew()
		{
			DataTable dt = DataProvider.CreateParentDataTable();
			DataView dv = new DataView(dt);

			// AllowNew - default value
			Assert.AreEqual(true , dv.AllowNew , "DV18");

			// AllowNew - true
			dv.AllowNew = true;
			Assert.AreEqual(true, dv.AllowNew , "DV19");

			// AllowNew - false
			dv.AllowNew = false;
			Assert.AreEqual(false, dv.AllowNew , "DV20");

			// AllowNew - exception
			try 
			{
				dv.AddNew();
				Assert.Fail("DV21: AddNew Failed to throw DataException");
			}
			catch (DataException) {}
			catch (AssertionException exc) {throw  exc;}
			catch (Exception exc)
			{
				Assert.Fail("DV22: AddNew. Wrong exception type. Got:" + exc);
			}

			dv.AllowNew=true;
			int RowsCount = dv.Count ;

			// AllowNew - exception
			dv.AddNew();
			Assert.AreEqual(RowsCount+1, dv.Count , "DV23");
		}

		[Test] public void ApplyDefaultSort()
		{
			DataTable dt = DataProvider.CreateParentDataTable();
			DataView dv = new DataView(dt);

			// ApplyDefaultSort - default value
			Assert.AreEqual(false , dv.ApplyDefaultSort , "DV24");

			// ApplyDefaultSort - true
			dv.ApplyDefaultSort = true;
			Assert.AreEqual(true, dv.ApplyDefaultSort , "DV25");

			// ApplyDefaultSort - false
			dv.ApplyDefaultSort = false;
			Assert.AreEqual(false, dv.ApplyDefaultSort , "DV26");
		}

		[Test] public void CopyTo()
		{
			//create the source datatable
			DataTable dt = DataProvider.CreateChildDataTable();

			//create the dataview for the table
			DataView dv = new DataView(dt);

			DataRowView[] drvExpected = null;
			DataRowView[] drvResult = null;

			// ------- Copy from Index=0
			drvExpected = new DataRowView[dv.Count];
			for (int i=0; i < dv.Count ;i++)
			{
				drvExpected[i] = dv[i];
			}

			drvResult = new DataRowView[dv.Count];
			// CopyTo from index 0
			dv.CopyTo(drvResult,0);
			Assert.AreEqual(drvResult, drvExpected , "DV27");

			// ------- Copy from Index=3
			drvExpected = new DataRowView[dv.Count+3];
			for (int i=0; i < dv.Count ;i++)
			{
				drvExpected[i+3] = dv[i];
			}

			drvResult = new DataRowView[dv.Count+3];
			// CopyTo from index 3
			dv.CopyTo(drvResult,3);
			Assert.AreEqual(drvResult , drvExpected , "DV28");

			// ------- Copy from Index=3,larger array
			drvExpected = new DataRowView[dv.Count+9];
			for (int i=0; i < dv.Count ;i++)
			{
				drvExpected[i+3] = dv[i];
			}

			drvResult = new DataRowView[dv.Count+9];
			// CopyTo from index 3,larger array
			dv.CopyTo(drvResult,3);
			Assert.AreEqual(drvResult, drvExpected , "DV29");

			// ------- CopyTo smaller array, check exception
			drvResult = new DataRowView[dv.Count-1];

			// CopyTo smaller array, check exception
			try 
			{
				dv.CopyTo(drvResult,0);
				Assert.Fail("DV30: CopyTo Failed to throw IndexOutOfRangeException");
			}
			catch (IndexOutOfRangeException) {}
			catch (AssertionException exc) {throw  exc;}
			catch (Exception exc)
			{
				Assert.Fail("DV31: CopyTo. Wrong exception type. Got:" + exc);
			}
		}

		[Test] public void Delete()
		{
			//create the source datatable
			DataTable dt = DataProvider.CreateChildDataTable();

			//create the dataview for the table
			DataView dv = new DataView(dt);

			int CountView = dv.Count ;
			int CountTable= dt.Rows.Count ;

			DataRowView drv = dv[0];

			// Delete - DataView Row Count
			dv.Delete(0);
			Assert.AreEqual(dv.Count , CountView-1, "DV32");

			// Delete - Table Row Count 
			Assert.AreEqual(dt.Rows.Count , CountTable, "DV33");

			// Delete - check table
			Assert.AreEqual(dt, drv.Row.Table, "DV34");
		}

		[Test] public void FindRows_ByKey()
		{
			DataRowView[] dvArr = null;

			//create the source datatable
			DataTable dt = DataProvider.CreateChildDataTable();

			//create the dataview for the table
			DataView dv = new DataView(dt);

			// FindRows ,no sort - exception
			try 
			{
				dvArr = dv.FindRows(3);
				Assert.Fail("DV35: FindRows Failed to throw ArgumentException");
			}
			catch (ArgumentException) {}
			catch (AssertionException exc) {throw  exc;}
			catch (Exception exc)
			{
				Assert.Fail("DV36: FindRows. Wrong exception type. Got:" + exc);
			}

			dv.Sort = "String1";
			// Find = wrong sort, can not find
			dvArr = dv.FindRows(3);
			Assert.AreEqual(0, dvArr.Length  , "DV37");

			dv.Sort = "ChildId";

			//get expected results
			DataRow[] drExpected = dt.Select("ChildId=3");

			// FindRows - check count
			dvArr = dv.FindRows(3);
			Assert.AreEqual(drExpected.Length , dvArr.Length, "DV38");

			// FindRows - check data

			//check that result is ok
			bool Succeed = true;
			for (int i=0; i<dvArr.Length ; i++)
			{
				Succeed = (int)dvArr[i]["ChildId"] == (int)drExpected [i]["ChildId"];
				if (!Succeed) break;
			}
			Assert.AreEqual(true, Succeed , "DV39");
		}

		[Test] public void FindRows_ByKeys()
		{
			DataRowView[] dvArr = null;

			//create the source datatable
			DataTable dt = DataProvider.CreateChildDataTable();

			//create the dataview for the table
			DataView dv = new DataView(dt);

			// FindRows ,no sort - exception
			try 
			{
				dvArr = dv.FindRows(new object[] {"3","3-String1"});
				Assert.Fail("DV40: FindRows Failed to throw ArgumentException");
			}
			catch (ArgumentException) {}
			catch (AssertionException exc) {throw  exc;}
			catch (Exception exc)
			{
				Assert.Fail("DV41: FindRows. Wrong exception type. Got:" + exc);
			}

			dv.Sort = "String1,ChildId";
			// Find = wrong sort, can not find
			dvArr = dv.FindRows(new object[] {"3","3-String1"});
			Assert.AreEqual(0, dvArr.Length  , "DV42");

			dv.Sort = "ChildId,String1";

			//get expected results
			DataRow[] drExpected = dt.Select("ChildId=3 and String1='3-String1'");

			// FindRows - check count
			dvArr = dv.FindRows(new object[] {"3","3-String1"});
			Assert.AreEqual(drExpected.Length , dvArr.Length, "DV43");

			// FindRows - check data

			//check that result is ok
			bool Succeed = true;
			for (int i=0; i<dvArr.Length ; i++)
			{
				Succeed = (int)dvArr[i]["ChildId"] == (int)drExpected [i]["ChildId"];
				if (!Succeed) break;
			}
			Assert.AreEqual(true, Succeed , "DV44");
		}

		//Activate This Construntor to log All To Standard output
		//public TestClass():base(true){}

		//Activate this constructor to log Failures to a log file
		//public TestClass(System.IO.TextWriter tw):base(tw, false){}

		//Activate this constructor to log All to a log file
		//public TestClass(System.IO.TextWriter tw):base(tw, true){}

		//BY DEFAULT LOGGING IS DONE TO THE STANDARD OUTPUT ONLY FOR FAILURES

		[Test] public void Find_ByObject()
		{
			int FindResult,ExpectedResult=-1;

			//create the source datatable
			DataTable dt = DataProvider.CreateParentDataTable();

			//create the dataview for the table
			DataView dv = new DataView(dt);

			for (int i=0; i<dt.Rows.Count ; i++)
			{
				if ((int)dt.Rows[i]["ParentId"] == 3)
				{
					ExpectedResult = i;
					break;
				}
			}

			// Find ,no sort - exception
			try 
			{
				FindResult = dv.Find("3");
				Assert.Fail("DV45: Find Failed to throw ArgumentException");
			}
			catch (ArgumentException) {}
			catch (AssertionException exc) {throw  exc;}
			catch (Exception exc)
			{
				Assert.Fail("DV46: Find. Wrong exception type. Got:" + exc);
			}

			dv.Sort = "String1";
			// Find = wrong sort, can not find
			FindResult = dv.Find("3");
			Assert.AreEqual(-1, FindResult , "DV47");

			dv.Sort = "ParentId";
			// Find 
			FindResult = dv.Find("3");
			Assert.AreEqual(ExpectedResult, FindResult , "DV48");
		}

		[Test] public void Find_ByArray()
		{
			int FindResult,ExpectedResult=-1;

			//create the source datatable
			DataTable dt = DataProvider.CreateParentDataTable();

			//create the dataview for the table
			DataView dv = new DataView(dt);

			for (int i=0; i<dt.Rows.Count ; i++)
			{
				if ((int)dt.Rows[i]["ParentId"] == 3 && dt.Rows[i]["String1"].ToString() == "3-String1")
				{
					ExpectedResult = i;
					break;
				}
			}

			// Find ,no sort - exception
			try 
			{
				FindResult = dv.Find(new object[] {"3","3-String1"});
				Assert.Fail("DV49: Find Failed to throw ArgumentException");
			}
			catch (ArgumentException) {}
			catch (AssertionException exc) {throw  exc;}
			catch (Exception exc)
			{
				Assert.Fail("DV50: Find. Wrong exception type. Got:" + exc);
			}

			dv.Sort = "String1,ParentId";
			// Find = wrong sort, can not find
			FindResult = dv.Find(new object[] {"3","3-String1"});
			Assert.AreEqual(-1, FindResult , "DV51");

			dv.Sort = "ParentId,String1";
			// Find 
			FindResult = dv.Find(new object[] {"3","3-String1"});
			Assert.AreEqual(ExpectedResult, FindResult , "DV52");
		}

		//Activate This Construntor to log All To Standard output
		//public TestClass():base(true){}

		//Activate this constructor to log Failures to a log file
		//public TestClass(System.IO.TextWriter tw):base(tw, false){}

		//Activate this constructor to log All to a log file
		//public TestClass(System.IO.TextWriter tw):base(tw, true){}

		//BY DEFAULT LOGGING IS DONE TO THE STANDARD OUTPUT ONLY FOR FAILURES

		[Test] public void GetEnumerator()
		{
			//create the source datatable
			DataTable dt = DataProvider.CreateChildDataTable();

			//create the dataview for the table
			DataView dv = new DataView(dt);

			System.Collections.IEnumerator ienm = null;

			// GetEnumerator != null
			ienm = dv.GetEnumerator();
			Assert.AreEqual(true, ienm != null, "DV53");

			int i=0;
			while (ienm.MoveNext() )
			{
				// Check item i
				Assert.AreEqual(dv[i], (DataRowView)ienm.Current , "DV54");
				i++;
			}
		}

		[Test] public void Item()
		{
			//create the source datatable
			DataTable dt = DataProvider.CreateParentDataTable();

			//create the dataview for the table
			DataView dv = new DataView(dt);

			// DataView Item 0
			Assert.AreEqual(dv[0].Row, dt.Rows[0] , "DV55");

			// DataView Item 4
			Assert.AreEqual(dv[4].Row, dt.Rows[4] , "DV56");

			dv.RowFilter="ParentId in (1,3,6)";

			// DataView Item 0,DataTable with filter
			Assert.AreEqual(dv[1].Row, dt.Rows[2] , "DV57");
		}

		[Test] public void ListChanged()
		{
			DataTable dt = DataProvider.CreateParentDataTable();
			DataView dv = new DataView(dt);

			//add event handler
			dv.ListChanged +=new System.ComponentModel.ListChangedEventHandler(dv_ListChanged);

			// ----- Change Value ---------
			evProp = null;
			// change value - Event raised
			dv[1]["String1"] = "something";
			Assert.AreEqual(true , evProp!=null , "DV58");
			// change value - ListChangedType
			Assert.AreEqual(System.ComponentModel.ListChangedType.ItemChanged, evProp.lstType , "DV59");
			// change value - NewIndex
			Assert.AreEqual(1, evProp.NewIndex, "DV60");
			// change value - OldIndex
			Assert.AreEqual(-1, evProp.OldIndex , "DV61");

			// ----- Add New ---------
			evProp = null;
			// Add New  - Event raised
			dv.AddNew();
			Assert.AreEqual(true , evProp!=null , "DV62");
			// Add New  - ListChangedType
			Assert.AreEqual(System.ComponentModel.ListChangedType.ItemAdded , evProp.lstType , "DV63");
			// Add New  - NewIndex
			Assert.AreEqual(6, evProp.NewIndex, "DV64");
			// Add New  - OldIndex
			Assert.AreEqual(-1, evProp.OldIndex , "DV65");

			// ----- Sort ---------
			evProp = null;
			// sort  - Event raised
			dv.Sort = "ParentId Desc";
			Assert.AreEqual(true , evProp!=null , "DV66");
			// sort - ListChangedType
			Assert.AreEqual(System.ComponentModel.ListChangedType.Reset , evProp.lstType , "DV67");
			// sort - NewIndex
			Assert.AreEqual(-1, evProp.NewIndex, "DV68");
			// sort - OldIndex
			Assert.AreEqual(-1, evProp.OldIndex , "DV69");

			//ListChangedType - this was not checked
			//Move
			//PropertyDescriptorAdded - A PropertyDescriptor was added, which changed the schema. 
			//PropertyDescriptorChanged - A PropertyDescriptor was changed, which changed the schema. 
			//PropertyDescriptorDeleted 
		}

		private void dv_ListChanged(object sender, System.ComponentModel.ListChangedEventArgs e)
		{
			evProp = new EventProperties();	
			evProp.lstType = e.ListChangedType;
			evProp.NewIndex = e.NewIndex;
			evProp.OldIndex = e.OldIndex; 
		}

		[Test] public void RowFilter()
		{
			//note: this test does not check all the possible row filter expression. this is done in DataTable.Select method.
			// this test also check DataView.Count property

			DataRowView[] drvResult = null;
			System.Collections.ArrayList al = new System.Collections.ArrayList();

			//create the source datatable
			DataTable dt = DataProvider.CreateChildDataTable();

			//create the dataview for the table
			DataView dv = new DataView(dt);

			//-------------------------------------------------------------
			//Get excpected result 
			al.Clear();
			foreach (DataRow dr in dt.Rows ) 
			{
				if ((int)dr["ChildId"] == 1)
				{
					al.Add(dr);
				}
			}

			// RowFilter = 'ChildId=1', check count
			dv.RowFilter = "ChildId=1";
			Assert.AreEqual(al.Count , dv.Count , "DV70");

			// RowFilter = 'ChildId=1', check rows
			drvResult = new DataRowView[dv.Count];
			dv.CopyTo(drvResult,0);
			//check that the filterd rows exists
			bool Succeed = true;
			for (int i=0; i<drvResult.Length ; i++)
			{
				Succeed = al.Contains(drvResult[i].Row);
				if (!Succeed) break;
			}
			Assert.AreEqual(true, Succeed , "DV71");
			//-------------------------------------------------------------

			//-------------------------------------------------------------
			//Get excpected result 
			al.Clear();
			foreach (DataRow dr in dt.Rows ) 
				if ((int)dr["ChildId"] == 1 && dr["String1"].ToString() == "1-String1" ) 
					al.Add(dr);

			// RowFilter - ChildId=1 and String1='1-String1'
			dv.RowFilter = "ChildId=1 and String1='1-String1'";
			Assert.AreEqual(al.Count , dv.Count , "DV72");

			// RowFilter = ChildId=1 and String1='1-String1', check rows
			drvResult = new DataRowView[dv.Count];
			dv.CopyTo(drvResult,0);
			//check that the filterd rows exists
			Succeed = true;
			for (int i=0; i<drvResult.Length ; i++)
			{
				Succeed = al.Contains(drvResult[i].Row);
				if (!Succeed) break;
			}
			Assert.AreEqual(true, Succeed , "DV73");
			//-------------------------------------------------------------

			//EvaluateException
			// RowFilter - check EvaluateException
			try 
			{
				dv.RowFilter = "Col=1";
				Assert.Fail("DV74: RowFilter Failed to throw EvaluateException");
			}
			catch (EvaluateException) {}
			catch (AssertionException exc) {throw  exc;}
			catch (Exception exc)
			{
				Assert.Fail("DV75: RowFilter. Wrong exception type. Got:" + exc);
			}

			//SyntaxErrorException 1
			// RowFilter - check SyntaxErrorException 1
			try 
			{
				dv.RowFilter = "sum('something')";
				Assert.Fail("DV76: RowFilter Failed to throw SyntaxErrorException");
			}
			catch (SyntaxErrorException) {}
			catch (AssertionException exc) {throw  exc;}
			catch (Exception exc)
			{
				Assert.Fail("DV77: RowFilter. Wrong exception type. Got:" + exc);
			}

			//SyntaxErrorException 2
			// RowFilter - check SyntaxErrorException 2
			try 
			{
				dv.RowFilter = "HH**!";
				Assert.Fail("DV78: RowFilter Failed to throw SyntaxErrorException");
			}
			catch (SyntaxErrorException) {}
			catch (AssertionException exc) {throw  exc;}
			catch (Exception exc)
			{
				Assert.Fail("DV79: RowFilter. Wrong exception type. Got:" + exc);
			}
		}

		[Test] public void RowStateFilter()
		{
			/*
				Added			A new row. 4 
				CurrentRows		Current rows including unchanged, new, and modified rows. 22 
				Deleted			A deleted row. 8 
				ModifiedCurrent A current version, which is a modified version of original data (see ModifiedOriginal). 16 
				ModifiedOriginal The original version (although it has since been modified and is available as ModifiedCurrent). 32 
				None			None. 0 
				OriginalRows	Original rows including unchanged and deleted rows. 42 
				Unchanged		An unchanged row. 2 
			 */

			//DataRowView[] drvResult = null;
			System.Collections.ArrayList al = new System.Collections.ArrayList();

			DataTable dt = DataProvider.CreateParentDataTable();

			//create the dataview for the table
			DataView dv = new DataView(dt);

			DataRow[]  drResult;

			dt.Rows[0].Delete();
			dt.Rows[1]["ParentId"] = 1;
			dt.Rows[2]["ParentId"] = 1;
			dt.Rows[3].Delete();
			dt.Rows.Add(new object[] {1,"A","B"});
			dt.Rows.Add(new object[] {1,"C","D"});
			dt.Rows.Add(new object[] {1,"E","F"});

			//---------- Added -------- 
			dv.RowStateFilter = DataViewRowState.Added ;
			drResult = GetResultRows(dt,DataRowState.Added);
			// Added
			Assert.AreEqual(true , CompareSortedRowsByParentId(dv,drResult), "DV80");

			//---------- CurrentRows -------- 
			dv.RowStateFilter = DataViewRowState.CurrentRows ;
			drResult = GetResultRows(dt,DataRowState.Unchanged | DataRowState.Added  | DataRowState.Modified );
			// CurrentRows
			Assert.AreEqual(true , CompareSortedRowsByParentId(dv,drResult), "DV81");

			//---------- ModifiedCurrent -------- 
			dv.RowStateFilter = DataViewRowState.ModifiedCurrent  ;
			drResult = GetResultRows(dt,DataRowState.Modified );
			// ModifiedCurrent
			Assert.AreEqual(true , CompareSortedRowsByParentId(dv,drResult) , "DV82");

			//---------- ModifiedOriginal -------- 
			dv.RowStateFilter = DataViewRowState.ModifiedOriginal   ;
			drResult = GetResultRows(dt,DataRowState.Modified );
			// ModifiedOriginal
			Assert.AreEqual(true , CompareSortedRowsByParentId(dv,drResult) , "DV83");

			//---------- Deleted -------- 
			dv.RowStateFilter = DataViewRowState.Deleted ;
			drResult = GetResultRows(dt,DataRowState.Deleted );
			// Deleted
			Assert.AreEqual(true , CompareSortedRowsByParentId(dv,drResult), "DV84");
			/*
					//---------- OriginalRows -------- 
					dv.RowStateFilter = DataViewRowState.OriginalRows ;
					drResult = GetResultRows(dt,DataRowState.Unchanged | DataRowState.Deleted );
						// OriginalRows
						Assert.AreEqual(true , CompareSortedRowsByParentId(dv,drResult), "DV85");
			*/
		}

		private DataRow[] GetResultRows(DataTable dt,DataRowState State)
		{
			//get expected rows
			System.Collections.ArrayList al = new System.Collections.ArrayList();
			DataRowVersion drVer = DataRowVersion.Current;

			//From MSDN -	The row the default version for the current DataRowState.
			//				For a DataRowState value of Added, Modified or Current, 
			//				the default version is Current. 
			//				For a DataRowState of Deleted, the version is Original.
			//				For a DataRowState value of Detached, the version is Proposed.

			if (	((State & DataRowState.Added)		> 0)  
				| ((State & DataRowState.Modified)	> 0)  
				| ((State & DataRowState.Unchanged)	> 0) ) 
				drVer = DataRowVersion.Current;
			if ( (State & DataRowState.Deleted)		> 0
				| (State & DataRowState.Detached)	> 0 )  
				drVer = DataRowVersion.Original; 

			foreach (DataRow dr in dt.Rows )
			{
				if ( dr.HasVersion(drVer) 
					//&& ((int)dr["ParentId", drVer] == 1) 
					&& ((dr.RowState & State) > 0 ) 
					)
					al.Add(dr);
			}
			DataRow[] result = (DataRow[])al.ToArray((typeof(DataRow)));
			return result; 
		}

		private bool CompareSortedRowsByParentId(DataView dv, DataRow[] drTable)
		{
			if (dv.Count != drTable.Length) throw new Exception("DataRows[] length are different");

			//comparing the rows by using columns ParentId and ChildId
			if ((dv.RowStateFilter & DataViewRowState.Deleted) > 0)
			{
				for (int i=0; i<dv.Count ; i++)
				{
					if (dv[i].Row["ParentId",DataRowVersion.Original ].ToString() != drTable[i]["ParentId",DataRowVersion.Original].ToString()) 
						return false;
				}
			}
			else
			{
				for (int i=0; i<dv.Count ; i++)
				{
					if (dv[i].Row["ParentId"].ToString() != drTable[i]["ParentId"].ToString()) 
						return false;
				}
			}
			return true;
		}

		[Test] public void Sort()
		{
			DataRow[] drArrTable;

			//create the source datatable
			DataTable dt = DataProvider.CreateChildDataTable();

			//create the dataview for the table
			DataView dv = new DataView(dt);

			dv.Sort = "ParentId";
			drArrTable = dt.Select("","ParentId");
			// sort = ParentId
			Assert.AreEqual(true, CompareSortedRowsByParentAndChildId(dv,drArrTable), "DV86");

			dv.Sort = "ChildId";
			drArrTable = dt.Select("","ChildId");
			// sort = ChildId
			Assert.AreEqual(true, CompareSortedRowsByParentAndChildId(dv,drArrTable), "DV87");

			dv.Sort = "ParentId Desc, ChildId";
			drArrTable = dt.Select("","ParentId Desc, ChildId");
			// sort = ParentId Desc, ChildId
			Assert.AreEqual(true, CompareSortedRowsByParentAndChildId(dv,drArrTable), "DV88");

			dv.Sort = "ChildId Asc, ParentId";
			drArrTable = dt.Select("","ChildId Asc, ParentId");
			// sort = ChildId Asc, ParentId
			Assert.AreEqual(true, CompareSortedRowsByParentAndChildId(dv,drArrTable), "DV89");

			dv.Sort = "ChildId Asc, ChildId Desc";
			drArrTable = dt.Select("","ChildId Asc, ChildId Desc");
			// sort = ChildId Asc, ChildId Desc
			Assert.AreEqual(true, CompareSortedRowsByParentAndChildId(dv,drArrTable), "DV90");

			// IndexOutOfRangeException - 1
			try 
			{
				dv.Sort = "something";
				Assert.Fail("DV91: Sort Failed to throw IndexOutOfRangeException");
			}
			catch (IndexOutOfRangeException) {}
			catch (AssertionException exc) {throw  exc;}
			catch (Exception exc)
			{
				Assert.Fail("DV92: Sort. Wrong exception type. Got:" + exc);
			}

			// IndexOutOfRangeException - 2
			try 
			{
				dv.Sort = "ColumnId Desc Asc";
				Assert.Fail("DV93: Sort Failed to throw IndexOutOfRangeException");
			}
			catch (IndexOutOfRangeException) {}
			catch (AssertionException exc) {throw  exc;}
			catch (Exception exc)
			{
				Assert.Fail("DV94: Sort. Wrong exception type. Got:" + exc);
			}

			// IndexOutOfRangeException - 3
			try 
			{
				dv.Sort = "ColumnId blabla";
				Assert.Fail("DV95: Sort Failed to throw IndexOutOfRangeException");
			}
			catch (IndexOutOfRangeException) {}
			catch (AssertionException exc) {throw  exc;}
			catch (Exception exc)
			{
				Assert.Fail("DV96: Sort. Wrong exception type. Got:" + exc);
			}
		}

		private bool CompareSortedRowsByParentAndChildId(DataView dv, DataRow[] drTable)
		{
			if (dv.Count != drTable.Length) throw new Exception("DataRows[] length are different");

			//comparing the rows by using columns ParentId and ChildId
			for (int i=0; i<dv.Count ; i++)
			{
				if (	dv[i].Row["ParentId"].ToString() != drTable[i]["ParentId"].ToString() 
					&& 
					dv[i].Row["ChildId"].ToString() != drTable[i]["ChildId"].ToString())
					return false;
			}
			return true;
		}

		[Test] public void Table()
		{
			DataTable dt = new DataTable();
			DataView dv = new DataView();

			// DataTable=null
			Assert.AreEqual(null , dv.Table , "DV97");

			// DataException - bind to table with no name
			try 
			{
				dv.Table = dt;
				Assert.Fail("DV98: Table Failed to throw DataException");
			}
			catch (DataException) {}
			catch (AssertionException exc) {throw  exc;}
			catch (Exception exc)
			{
				Assert.Fail("DV99: Table. Wrong exception type. Got:" + exc);
			}

			dt.TableName = "myTable";
			// DataTable!=null
			dv.Table = dt;
			Assert.AreEqual(dt, dv.Table , "DV100");

			// assign null to DataTable
			dv.Table = null; 
			Assert.AreEqual(null, dv.Table , "DV101");
		}

		[Test] public void ctor_Empty()
		{
			DataView dv; 
			dv = new DataView();

			// ctor
			Assert.AreEqual(false, dv == null, "DV102");
		}

		[Test] public void ctor_DataTable()
		{
			DataView dv = null; 
			DataTable dt = new DataTable("myTable");

			// ctor
			dv = new DataView(dt);
			Assert.AreEqual(false, dv == null, "DV103");

			// ctor - table
			Assert.AreEqual(dt , dv.Table  , "DV104");
		}

		[Test] public void ctor_ExpectedExceptions()
		{
			DataView dv = null; 
			DataTable dt = new DataTable("myTable");

			// ctor - missing column CutomerID Exception
			try 
			{
				//exception: System.Data.EvaluateException: Cannot find column [CustomerId]
				dv = new DataView(dt,"CustomerId > 100","Age",DataViewRowState.Added );
				Assert.Fail("DV105: DataView ctor Failed to throw EvaluateException or IndexOutOfRangeException");
			}
			catch (EvaluateException) {}
			catch (IndexOutOfRangeException) {}
			catch (AssertionException exc) {throw  exc;}
			catch (Exception exc)
			{
				Assert.Fail("DV106: DataView ctor. Wrong exception type. Got:" + exc);
			}

			dt.Columns.Add(new DataColumn("CustomerId"));

			// ctor - missing column Age Exception
			try 
			{
				//exception: System.Data.EvaluateException: Cannot find column [Age]
				dv = new DataView(dt,"CustomerId > 100","Age",DataViewRowState.Added );
				Assert.Fail("DV107: DataView ctor Failed to throw IndexOutOfRangeException");
			}
			catch (IndexOutOfRangeException) {}
			catch (AssertionException exc) {throw  exc;}
			catch (Exception exc)
			{
				Assert.Fail("DV108: DataView ctor. Wrong exception type. Got:" + exc);
			}
		}

		[Test] public void ctor_Complex()
		{
			DataView dv = null; 
			DataTable dt = new DataTable("myTable");

			dt.Columns.Add(new DataColumn("CustomerId"));
			dt.Columns.Add(new DataColumn("Age"));

			// ctor
			dv = new DataView(dt,"CustomerId > 100","Age",DataViewRowState.Added );
			Assert.AreEqual(false , dv == null  , "DV109");

			// ctor - table
			Assert.AreEqual(dt , dv.Table  , "DV110");

			// ctor - RowFilter
			Assert.AreEqual("CustomerId > 100" , dv.RowFilter , "DV111");

			// ctor - Sort
			Assert.AreEqual("Age" , dv.Sort, "DV112");

			// ctor - RowStateFilter 
			Assert.AreEqual(DataViewRowState.Added , dv.RowStateFilter , "DV113");
		}
	}
}
