﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MilSpace.DataAccess.Definition
{
	using System.Data.Linq;
	using System.Data.Linq.Mapping;
	using System.Data;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Linq;
	using System.Linq.Expressions;
	using System.ComponentModel;
	using System;
	
	
	[global::System.Data.Linq.Mapping.DatabaseAttribute(Name="DemPreparation")]
	public partial class DemPreparationContext : System.Data.Linq.DataContext
	{
		
		private static System.Data.Linq.Mapping.MappingSource mappingSource = new AttributeMappingSource();
		
    #region Extensibility Method Definitions
    partial void OnCreated();
    partial void InsertS1PairCoherence(S1PairCoherence instance);
    partial void UpdateS1PairCoherence(S1PairCoherence instance);
    partial void DeleteS1PairCoherence(S1PairCoherence instance);
    partial void InsertS1Sources(S1Sources instance);
    partial void UpdateS1Sources(S1Sources instance);
    partial void DeleteS1Sources(S1Sources instance);
    partial void InsertS1SentinelProduct(S1SentinelProduct instance);
    partial void UpdateS1SentinelProduct(S1SentinelProduct instance);
    partial void DeleteS1SentinelProduct(S1SentinelProduct instance);
    partial void InsertS1TilesCoverage(S1TilesCoverage instance);
    partial void UpdateS1TilesCoverage(S1TilesCoverage instance);
    partial void DeleteS1TilesCoverage(S1TilesCoverage instance);
    #endregion
		
		public DemPreparationContext() : 
				base(global::MilSpace.DataAccess.Properties.Settings.Default.DemPreparationConnectionString, mappingSource)
		{
			OnCreated();
		}
		
		public DemPreparationContext(string connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public DemPreparationContext(System.Data.IDbConnection connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public DemPreparationContext(string connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public DemPreparationContext(System.Data.IDbConnection connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		internal System.Data.Linq.Table<S1PairCoherence> S1PairCoherences
		{
			get
			{
				return this.GetTable<S1PairCoherence>();
			}
		}
		
		internal System.Data.Linq.Table<S1Sources> S1Sources
		{
			get
			{
				return this.GetTable<S1Sources>();
			}
		}
		
		public System.Data.Linq.Table<S1SentinelProduct> S1SentinelProducts
		{
			get
			{
				return this.GetTable<S1SentinelProduct>();
			}
		}
		
		internal System.Data.Linq.Table<S1TilesCoverage> S1TilesCoverages
		{
			get
			{
				return this.GetTable<S1TilesCoverage>();
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.MilSpace_S1PairCoh")]
	internal partial class S1PairCoherence : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _idrow;
		
		private string _idSceneBase;
		
		private string _idScentSlave;
		
		private System.Nullable<double> _fmean;
		
		private System.Nullable<double> _fdeviation;
		
		private System.Nullable<double> _fmin;
		
		private System.Nullable<double> _fmax;
		
		private System.Nullable<System.DateTime> _dto;
		
		private string _soper;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnidrowChanging(int value);
    partial void OnidrowChanged();
    partial void OnidSceneBaseChanging(string value);
    partial void OnidSceneBaseChanged();
    partial void OnidScentSlaveChanging(string value);
    partial void OnidScentSlaveChanged();
    partial void OnfmeanChanging(System.Nullable<double> value);
    partial void OnfmeanChanged();
    partial void OnfdeviationChanging(System.Nullable<double> value);
    partial void OnfdeviationChanged();
    partial void OnfminChanging(System.Nullable<double> value);
    partial void OnfminChanged();
    partial void OnfmaxChanging(System.Nullable<double> value);
    partial void OnfmaxChanged();
    partial void OndtoChanging(System.Nullable<System.DateTime> value);
    partial void OndtoChanged();
    partial void OnsoperChanging(string value);
    partial void OnsoperChanged();
    #endregion
		
		public S1PairCoherence()
		{
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_idrow", AutoSync=AutoSync.OnInsert, DbType="Int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int idrow
		{
			get
			{
				return this._idrow;
			}
			set
			{
				if ((this._idrow != value))
				{
					this.OnidrowChanging(value);
					this.SendPropertyChanging();
					this._idrow = value;
					this.SendPropertyChanged("idrow");
					this.OnidrowChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_idSceneBase", DbType="NVarChar(128)")]
		public string idSceneBase
		{
			get
			{
				return this._idSceneBase;
			}
			set
			{
				if ((this._idSceneBase != value))
				{
					this.OnidSceneBaseChanging(value);
					this.SendPropertyChanging();
					this._idSceneBase = value;
					this.SendPropertyChanged("idSceneBase");
					this.OnidSceneBaseChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_idScentSlave", DbType="NVarChar(128)")]
		public string idScentSlave
		{
			get
			{
				return this._idScentSlave;
			}
			set
			{
				if ((this._idScentSlave != value))
				{
					this.OnidScentSlaveChanging(value);
					this.SendPropertyChanging();
					this._idScentSlave = value;
					this.SendPropertyChanged("idScentSlave");
					this.OnidScentSlaveChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_fmean", DbType="Float")]
		public System.Nullable<double> fmean
		{
			get
			{
				return this._fmean;
			}
			set
			{
				if ((this._fmean != value))
				{
					this.OnfmeanChanging(value);
					this.SendPropertyChanging();
					this._fmean = value;
					this.SendPropertyChanged("fmean");
					this.OnfmeanChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_fdeviation", DbType="Float")]
		public System.Nullable<double> fdeviation
		{
			get
			{
				return this._fdeviation;
			}
			set
			{
				if ((this._fdeviation != value))
				{
					this.OnfdeviationChanging(value);
					this.SendPropertyChanging();
					this._fdeviation = value;
					this.SendPropertyChanged("fdeviation");
					this.OnfdeviationChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_fmin", DbType="Float")]
		public System.Nullable<double> fmin
		{
			get
			{
				return this._fmin;
			}
			set
			{
				if ((this._fmin != value))
				{
					this.OnfminChanging(value);
					this.SendPropertyChanging();
					this._fmin = value;
					this.SendPropertyChanged("fmin");
					this.OnfminChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_fmax", DbType="Float")]
		public System.Nullable<double> fmax
		{
			get
			{
				return this._fmax;
			}
			set
			{
				if ((this._fmax != value))
				{
					this.OnfmaxChanging(value);
					this.SendPropertyChanging();
					this._fmax = value;
					this.SendPropertyChanged("fmax");
					this.OnfmaxChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_dto", DbType="DateTime")]
		public System.Nullable<System.DateTime> dto
		{
			get
			{
				return this._dto;
			}
			set
			{
				if ((this._dto != value))
				{
					this.OndtoChanging(value);
					this.SendPropertyChanging();
					this._dto = value;
					this.SendPropertyChanged("dto");
					this.OndtoChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_soper", DbType="NVarChar(64)")]
		public string soper
		{
			get
			{
				return this._soper;
			}
			set
			{
				if ((this._soper != value))
				{
					this.OnsoperChanging(value);
					this.SendPropertyChanging();
					this._soper = value;
					this.SendPropertyChanged("soper");
					this.OnsoperChanged();
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.MilSpace_S1SRC")]
	internal partial class S1Sources : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _idrow;
		
		private string _idscene;
		
		private string _extend;
		
		private System.Nullable<System.DateTime> _dttime;
		
		private System.Nullable<int> _nburst;
		
		private System.Nullable<int> _norbit;
		
		private System.Nullable<System.DateTime> _dto;
		
		private string _soper;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnidrowChanging(int value);
    partial void OnidrowChanged();
    partial void OnidsceneChanging(string value);
    partial void OnidsceneChanged();
    partial void OnextendChanging(string value);
    partial void OnextendChanged();
    partial void OndttimeChanging(System.Nullable<System.DateTime> value);
    partial void OndttimeChanged();
    partial void OnnburstChanging(System.Nullable<int> value);
    partial void OnnburstChanged();
    partial void OnnorbitChanging(System.Nullable<int> value);
    partial void OnnorbitChanged();
    partial void OndtoChanging(System.Nullable<System.DateTime> value);
    partial void OndtoChanged();
    partial void OnsoperChanging(string value);
    partial void OnsoperChanged();
    #endregion
		
		public S1Sources()
		{
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_idrow", AutoSync=AutoSync.OnInsert, DbType="Int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int idrow
		{
			get
			{
				return this._idrow;
			}
			set
			{
				if ((this._idrow != value))
				{
					this.OnidrowChanging(value);
					this.SendPropertyChanging();
					this._idrow = value;
					this.SendPropertyChanged("idrow");
					this.OnidrowChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_idscene", DbType="NVarChar(128)")]
		public string idscene
		{
			get
			{
				return this._idscene;
			}
			set
			{
				if ((this._idscene != value))
				{
					this.OnidsceneChanging(value);
					this.SendPropertyChanging();
					this._idscene = value;
					this.SendPropertyChanged("idscene");
					this.OnidsceneChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_extend", DbType="NVarChar(255)")]
		public string extend
		{
			get
			{
				return this._extend;
			}
			set
			{
				if ((this._extend != value))
				{
					this.OnextendChanging(value);
					this.SendPropertyChanging();
					this._extend = value;
					this.SendPropertyChanged("extend");
					this.OnextendChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_dttime", DbType="DateTime")]
		public System.Nullable<System.DateTime> dttime
		{
			get
			{
				return this._dttime;
			}
			set
			{
				if ((this._dttime != value))
				{
					this.OndttimeChanging(value);
					this.SendPropertyChanging();
					this._dttime = value;
					this.SendPropertyChanged("dttime");
					this.OndttimeChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_nburst", DbType="Int")]
		public System.Nullable<int> nburst
		{
			get
			{
				return this._nburst;
			}
			set
			{
				if ((this._nburst != value))
				{
					this.OnnburstChanging(value);
					this.SendPropertyChanging();
					this._nburst = value;
					this.SendPropertyChanged("nburst");
					this.OnnburstChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_norbit", DbType="Int")]
		public System.Nullable<int> norbit
		{
			get
			{
				return this._norbit;
			}
			set
			{
				if ((this._norbit != value))
				{
					this.OnnorbitChanging(value);
					this.SendPropertyChanging();
					this._norbit = value;
					this.SendPropertyChanged("norbit");
					this.OnnorbitChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_dto", DbType="DateTime")]
		public System.Nullable<System.DateTime> dto
		{
			get
			{
				return this._dto;
			}
			set
			{
				if ((this._dto != value))
				{
					this.OndtoChanging(value);
					this.SendPropertyChanging();
					this._dto = value;
					this.SendPropertyChanged("dto");
					this.OndtoChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_soper", DbType="NVarChar(50)")]
		public string soper
		{
			get
			{
				return this._soper;
			}
			set
			{
				if ((this._soper != value))
				{
					this.OnsoperChanging(value);
					this.SendPropertyChanging();
					this._soper = value;
					this.SendPropertyChanged("soper");
					this.OnsoperChanged();
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.MilSpace_SentinelProduct")]
	public partial class S1SentinelProduct : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _Id;
		
		private string _Uuid;
		
		private string _Identifier;
		
		private System.DateTime _DateTime;
		
		private string _Instrument;
		
		private string _Footprint;
		
		private string _JTSfootprint;
		
		private string _PassDirection;
		
		private int _RelativeOrbit;
		
		private int _OrbitNumber;
		
		private int _SliceNumber;
		
		private string _Wkt;
		
		private string _sOper;
		
		private System.DateTime _Dto;
		
		private string _TilName;
		
		private bool _Downloaded;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnIdChanging(int value);
    partial void OnIdChanged();
    partial void OnUuidChanging(string value);
    partial void OnUuidChanged();
    partial void OnIdentifierChanging(string value);
    partial void OnIdentifierChanged();
    partial void OnDateTimeChanging(System.DateTime value);
    partial void OnDateTimeChanged();
    partial void OnInstrumentChanging(string value);
    partial void OnInstrumentChanged();
    partial void OnFootprintChanging(string value);
    partial void OnFootprintChanged();
    partial void OnJTSfootprintChanging(string value);
    partial void OnJTSfootprintChanged();
    partial void OnPassDirectionChanging(string value);
    partial void OnPassDirectionChanged();
    partial void OnRelativeOrbitChanging(int value);
    partial void OnRelativeOrbitChanged();
    partial void OnOrbitNumberChanging(int value);
    partial void OnOrbitNumberChanged();
    partial void OnSliceNumberChanging(int value);
    partial void OnSliceNumberChanged();
    partial void OnWktChanging(string value);
    partial void OnWktChanged();
    partial void OnsOperChanging(string value);
    partial void OnsOperChanged();
    partial void OnDtoChanging(System.DateTime value);
    partial void OnDtoChanged();
    partial void OnTileNameChanging(string value);
    partial void OnTileNameChanged();
    partial void OnDownloadedChanging(bool value);
    partial void OnDownloadedChanged();
    #endregion
		
		public S1SentinelProduct()
		{
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Id", DbType="Int NOT NULL", IsPrimaryKey=true)]
		public int Id
		{
			get
			{
				return this._Id;
			}
			set
			{
				if ((this._Id != value))
				{
					this.OnIdChanging(value);
					this.SendPropertyChanging();
					this._Id = value;
					this.SendPropertyChanged("Id");
					this.OnIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Uuid", DbType="NChar(20) NOT NULL", CanBeNull=false)]
		public string Uuid
		{
			get
			{
				return this._Uuid;
			}
			set
			{
				if ((this._Uuid != value))
				{
					this.OnUuidChanging(value);
					this.SendPropertyChanging();
					this._Uuid = value;
					this.SendPropertyChanged("Uuid");
					this.OnUuidChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Identifier", DbType="NChar(38) NOT NULL", CanBeNull=false)]
		public string Identifier
		{
			get
			{
				return this._Identifier;
			}
			set
			{
				if ((this._Identifier != value))
				{
					this.OnIdentifierChanging(value);
					this.SendPropertyChanging();
					this._Identifier = value;
					this.SendPropertyChanged("Identifier");
					this.OnIdentifierChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_DateTime", DbType="DateTime NOT NULL")]
		public System.DateTime DateTime
		{
			get
			{
				return this._DateTime;
			}
			set
			{
				if ((this._DateTime != value))
				{
					this.OnDateTimeChanging(value);
					this.SendPropertyChanging();
					this._DateTime = value;
					this.SendPropertyChanged("DateTime");
					this.OnDateTimeChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Instrument", DbType="NChar(20) NOT NULL", CanBeNull=false)]
		public string Instrument
		{
			get
			{
				return this._Instrument;
			}
			set
			{
				if ((this._Instrument != value))
				{
					this.OnInstrumentChanging(value);
					this.SendPropertyChanging();
					this._Instrument = value;
					this.SendPropertyChanged("Instrument");
					this.OnInstrumentChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Footprint", DbType="NVarChar(MAX) NOT NULL", CanBeNull=false)]
		public string Footprint
		{
			get
			{
				return this._Footprint;
			}
			set
			{
				if ((this._Footprint != value))
				{
					this.OnFootprintChanging(value);
					this.SendPropertyChanging();
					this._Footprint = value;
					this.SendPropertyChanged("Footprint");
					this.OnFootprintChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_JTSfootprint", DbType="NVarChar(MAX) NOT NULL", CanBeNull=false)]
		public string JTSfootprint
		{
			get
			{
				return this._JTSfootprint;
			}
			set
			{
				if ((this._JTSfootprint != value))
				{
					this.OnJTSfootprintChanging(value);
					this.SendPropertyChanging();
					this._JTSfootprint = value;
					this.SendPropertyChanged("JTSfootprint");
					this.OnJTSfootprintChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_PassDirection", DbType="NChar(10) NOT NULL", CanBeNull=false)]
		public string PassDirection
		{
			get
			{
				return this._PassDirection;
			}
			set
			{
				if ((this._PassDirection != value))
				{
					this.OnPassDirectionChanging(value);
					this.SendPropertyChanging();
					this._PassDirection = value;
					this.SendPropertyChanged("PassDirection");
					this.OnPassDirectionChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_RelativeOrbit", DbType="Int NOT NULL")]
		public int RelativeOrbit
		{
			get
			{
				return this._RelativeOrbit;
			}
			set
			{
				if ((this._RelativeOrbit != value))
				{
					this.OnRelativeOrbitChanging(value);
					this.SendPropertyChanging();
					this._RelativeOrbit = value;
					this.SendPropertyChanged("RelativeOrbit");
					this.OnRelativeOrbitChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_OrbitNumber", DbType="Int NOT NULL")]
		public int OrbitNumber
		{
			get
			{
				return this._OrbitNumber;
			}
			set
			{
				if ((this._OrbitNumber != value))
				{
					this.OnOrbitNumberChanging(value);
					this.SendPropertyChanging();
					this._OrbitNumber = value;
					this.SendPropertyChanged("OrbitNumber");
					this.OnOrbitNumberChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_SliceNumber", DbType="Int NOT NULL")]
		public int SliceNumber
		{
			get
			{
				return this._SliceNumber;
			}
			set
			{
				if ((this._SliceNumber != value))
				{
					this.OnSliceNumberChanging(value);
					this.SendPropertyChanging();
					this._SliceNumber = value;
					this.SendPropertyChanged("SliceNumber");
					this.OnSliceNumberChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Wkt", DbType="NVarChar(MAX) NOT NULL", CanBeNull=false)]
		public string Wkt
		{
			get
			{
				return this._Wkt;
			}
			set
			{
				if ((this._Wkt != value))
				{
					this.OnWktChanging(value);
					this.SendPropertyChanging();
					this._Wkt = value;
					this.SendPropertyChanged("Wkt");
					this.OnWktChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_sOper", DbType="NChar(20) NOT NULL", CanBeNull=false)]
		public string sOper
		{
			get
			{
				return this._sOper;
			}
			set
			{
				if ((this._sOper != value))
				{
					this.OnsOperChanging(value);
					this.SendPropertyChanging();
					this._sOper = value;
					this.SendPropertyChanged("sOper");
					this.OnsOperChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Dto", DbType="DateTime NOT NULL")]
		public System.DateTime Dto
		{
			get
			{
				return this._Dto;
			}
			set
			{
				if ((this._Dto != value))
				{
					this.OnDtoChanging(value);
					this.SendPropertyChanging();
					this._Dto = value;
					this.SendPropertyChanged("Dto");
					this.OnDtoChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_TilName", DbType="NChar(6) NOT NULL", CanBeNull=false)]
		public string TileName
		{
			get
			{
				return this._TilName;
			}
			set
			{
				if ((this._TilName != value))
				{
					this.OnTileNameChanging(value);
					this.SendPropertyChanging();
					this._TilName = value;
					this.SendPropertyChanged("TileName");
					this.OnTileNameChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Downloaded", DbType="Bit NOT NULL")]
		public bool Downloaded
		{
			get
			{
				return this._Downloaded;
			}
			set
			{
				if ((this._Downloaded != value))
				{
					this.OnDownloadedChanging(value);
					this.SendPropertyChanging();
					this._Downloaded = value;
					this.SendPropertyChanged("Downloaded");
					this.OnDownloadedChanged();
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.MilSpace_S1TilesCover")]
	internal partial class S1TilesCoverage : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _idrow;
		
		private string _QuaziTileName;
		
		private System.DateTime _Dto;
		
		private string _sOper;
		
		private string _DEMFilePath;
		
		private string _SceneName;
		
		private int _Status;
		
		private string _Wkt;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnidrowChanging(int value);
    partial void OnidrowChanged();
    partial void OnQuaziTileNameChanging(string value);
    partial void OnQuaziTileNameChanged();
    partial void OnDtoChanging(System.DateTime value);
    partial void OnDtoChanged();
    partial void OnsOperChanging(string value);
    partial void OnsOperChanged();
    partial void OnDEMFilePathChanging(string value);
    partial void OnDEMFilePathChanged();
    partial void OnSceneNameChanging(string value);
    partial void OnSceneNameChanged();
    partial void OnStatusChanging(int value);
    partial void OnStatusChanged();
    partial void OnWktChanging(string value);
    partial void OnWktChanged();
    #endregion
		
		public S1TilesCoverage()
		{
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_idrow", AutoSync=AutoSync.OnInsert, DbType="Int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int idrow
		{
			get
			{
				return this._idrow;
			}
			set
			{
				if ((this._idrow != value))
				{
					this.OnidrowChanging(value);
					this.SendPropertyChanging();
					this._idrow = value;
					this.SendPropertyChanged("idrow");
					this.OnidrowChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_QuaziTileName", DbType="NVarChar(50) NOT NULL", CanBeNull=false)]
		public string QuaziTileName
		{
			get
			{
				return this._QuaziTileName;
			}
			set
			{
				if ((this._QuaziTileName != value))
				{
					this.OnQuaziTileNameChanging(value);
					this.SendPropertyChanging();
					this._QuaziTileName = value;
					this.SendPropertyChanged("QuaziTileName");
					this.OnQuaziTileNameChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Dto", DbType="DateTime NOT NULL")]
		public System.DateTime Dto
		{
			get
			{
				return this._Dto;
			}
			set
			{
				if ((this._Dto != value))
				{
					this.OnDtoChanging(value);
					this.SendPropertyChanging();
					this._Dto = value;
					this.SendPropertyChanged("Dto");
					this.OnDtoChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_sOper", DbType="NVarChar(50) NOT NULL", CanBeNull=false)]
		public string sOper
		{
			get
			{
				return this._sOper;
			}
			set
			{
				if ((this._sOper != value))
				{
					this.OnsOperChanging(value);
					this.SendPropertyChanging();
					this._sOper = value;
					this.SendPropertyChanged("sOper");
					this.OnsOperChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_DEMFilePath", DbType="NVarChar(MAX)")]
		public string DEMFilePath
		{
			get
			{
				return this._DEMFilePath;
			}
			set
			{
				if ((this._DEMFilePath != value))
				{
					this.OnDEMFilePathChanging(value);
					this.SendPropertyChanging();
					this._DEMFilePath = value;
					this.SendPropertyChanged("DEMFilePath");
					this.OnDEMFilePathChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_SceneName", DbType="NChar(100) NOT NULL", CanBeNull=false)]
		public string SceneName
		{
			get
			{
				return this._SceneName;
			}
			set
			{
				if ((this._SceneName != value))
				{
					this.OnSceneNameChanging(value);
					this.SendPropertyChanging();
					this._SceneName = value;
					this.SendPropertyChanged("SceneName");
					this.OnSceneNameChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Status", DbType="Int NOT NULL")]
		public int Status
		{
			get
			{
				return this._Status;
			}
			set
			{
				if ((this._Status != value))
				{
					this.OnStatusChanging(value);
					this.SendPropertyChanging();
					this._Status = value;
					this.SendPropertyChanged("Status");
					this.OnStatusChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Wkt", DbType="NVarChar(MAX)", CanBeNull=false)]
		public string Wkt
		{
			get
			{
				return this._Wkt;
			}
			set
			{
				if ((this._Wkt != value))
				{
					this.OnWktChanging(value);
					this.SendPropertyChanging();
					this._Wkt = value;
					this.SendPropertyChanged("Wkt");
					this.OnWktChanged();
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}
#pragma warning restore 1591
