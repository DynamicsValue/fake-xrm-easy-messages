using Microsoft.Xrm.Sdk;

namespace DataverseEntities
{
    [System.Runtime.Serialization.DataContractAttribute()]
	[Microsoft.Xrm.Sdk.Client.EntityLogicalNameAttribute("principalobjectaccess")]
	[System.CodeDom.Compiler.GeneratedCodeAttribute("CrmSvcUtil", "7.0.0000.3543")]
	public partial class PrincipalObjectAccess : Microsoft.Xrm.Sdk.Entity, System.ComponentModel.INotifyPropertyChanging, System.ComponentModel.INotifyPropertyChanged
	{
		/// <summary>
		/// Default Constructor.
		/// </summary>
		public PrincipalObjectAccess() : 
				base(EntityLogicalName)
		{
		}
		
		public const string EntityLogicalName = "principalobjectaccess";
		
		public const int EntityTypeCode = 11;
		
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
		
		public event System.ComponentModel.PropertyChangingEventHandler PropertyChanging;
		
		private void OnPropertyChanged(string propertyName)
		{
			if ((PropertyChanged != null))
			{
				PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
			}
		}
		
		private void OnPropertyChanging(string propertyName)
		{
			if ((PropertyChanging != null))
			{
				PropertyChanging(this, new System.ComponentModel.PropertyChangingEventArgs(propertyName));
			}
		}
		
		[AttributeLogicalName("principalobjectaccessid")]
		public override System.Guid Id
		{
			get
			{
				return base.Id;
			}
			set
			{
				PrincipalObjectAccessId = value;
			}
		}
		
		[AttributeLogicalName("accessrightsmask")]
		public System.Nullable<int> AccessRightsMask
		{
			get
			{
				return GetAttributeValue<System.Nullable<int>>("accessrightsmask");
			}
			set
			{
				OnPropertyChanging("AccessRightsMask");
				SetAttributeValue("accessrightsmask", value);
				OnPropertyChanged("AccessRightsMask");
			}
		}
		
		[AttributeLogicalName("changedon")]
		public System.Nullable<System.DateTime> ChangedOn
		{
			get
			{
				return GetAttributeValue<System.Nullable<System.DateTime>>("changedon");
			}
			set
			{
				OnPropertyChanging("ChangedOn");
				SetAttributeValue("changedon", value);
				OnPropertyChanged("ChangedOn");
			}
		}
		
		[AttributeLogicalName("inheritedaccessrightsmask")]
		public System.Nullable<int> InheritedAccessRightsMask
		{
			get
			{
				return GetAttributeValue<System.Nullable<int>>("inheritedaccessrightsmask");
			}
			set
			{
				OnPropertyChanging("InheritedAccessRightsMask");
				SetAttributeValue("inheritedaccessrightsmask", value);
				OnPropertyChanged("InheritedAccessRightsMask");
			}
		}
		
		[AttributeLogicalName("objectid")]
		public System.Nullable<System.Guid> ObjectId
		{
			get
			{
				return GetAttributeValue<System.Nullable<System.Guid>>("objectid");
			}
		}
		
		[AttributeLogicalName("objecttypecode")]
		public string ObjectTypeCode
		{
			get
			{
				return GetAttributeValue<string>("objecttypecode");
			}
			set
			{
				OnPropertyChanging("ObjectTypeCode");
				SetAttributeValue("objecttypecode", value);
				OnPropertyChanged("ObjectTypeCode");
			}
		}
		
		[AttributeLogicalName("principalid")]
		public System.Nullable<System.Guid> PrincipalId
		{
			get
			{
				return GetAttributeValue<System.Nullable<System.Guid>>("principalid");
			}
		}
		
		/// <summary>
		/// Unikatowy identyfikator dostępu podmiotów do obiektów.
		/// </summary>
		[AttributeLogicalName("principalobjectaccessid")]
		public System.Nullable<System.Guid> PrincipalObjectAccessId
		{
			get
			{
				return GetAttributeValue<System.Nullable<System.Guid>>("principalobjectaccessid");
			}
			set
			{
				OnPropertyChanging("PrincipalObjectAccessId");
				SetAttributeValue("principalobjectaccessid", value);
				if (value.HasValue)
				{
					base.Id = value.Value;
				}
				else
				{
					base.Id = System.Guid.Empty;
				}
				OnPropertyChanged("PrincipalObjectAccessId");
			}
		}
		
		[AttributeLogicalName("principaltypecode")]
		public string PrincipalTypeCode
		{
			get
			{
				return GetAttributeValue<string>("principaltypecode");
			}
			set
			{
				OnPropertyChanging("PrincipalTypeCode");
				SetAttributeValue("principaltypecode", value);
				OnPropertyChanged("PrincipalTypeCode");
			}
		}
		
		/// <summary>
		/// Tylko do użytku wewnętrznego.
		/// </summary>
		[AttributeLogicalName("timezoneruleversionnumber")]
		public System.Nullable<int> TimeZoneRuleVersionNumber
		{
			get
			{
				return GetAttributeValue<System.Nullable<int>>("timezoneruleversionnumber");
			}
			set
			{
				OnPropertyChanging("TimeZoneRuleVersionNumber");
				SetAttributeValue("timezoneruleversionnumber", value);
				OnPropertyChanged("TimeZoneRuleVersionNumber");
			}
		}
		
		/// <summary>
		/// Kod strefy czasowej używanej przy tworzeniu rekordu.
		/// </summary>
		[AttributeLogicalName("utcconversiontimezonecode")]
		public System.Nullable<int> UTCConversionTimeZoneCode
		{
			get
			{
				return GetAttributeValue<System.Nullable<int>>("utcconversiontimezonecode");
			}
			set
			{
				OnPropertyChanging("UTCConversionTimeZoneCode");
				SetAttributeValue("utcconversiontimezonecode", value);
				OnPropertyChanged("UTCConversionTimeZoneCode");
			}
		}
		
		[AttributeLogicalName("versionnumber")]
		public System.Nullable<long> VersionNumber
		{
			get
			{
				return GetAttributeValue<System.Nullable<long>>("versionnumber");
			}
		}
	}
}