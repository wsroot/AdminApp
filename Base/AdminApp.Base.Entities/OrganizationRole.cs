//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由T4模板自动生成
//       生成时间 2018-10-26 15:54:04
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

using System;
namespace AdminApp.Base.Entities
{    
    /// <summary>
    /// 
    /// </summary>    
	public class OrganizationRole
    {
        
		/// <summary>
        /// 
        /// </summary>        
        public string Id { get; set; }
    
		/// <summary>
        /// 
        /// </summary>        
        public string OrganizationId { get; set; }
    
		/// <summary>
        /// 
        /// </summary>        
        public string Name { get; set; }
    
		/// <summary>
        /// 
        /// </summary>        
        public int InUse { get; set; }
    
		/// <summary>
        /// 
        /// </summary>        
        public string ApplicationId { get; set; }
    
		/// <summary>
        /// 
        /// </summary>        
        public string CreatedBy { get; set; }
    
		/// <summary>
        /// 
        /// </summary>        
        public DateTime CreatedTime { get; set; }
    
		/// <summary>
        /// 
        /// </summary>        
        public string ModifiedBy { get; set; }
    
		/// <summary>
        /// 
        /// </summary>        
        public DateTime? ModifiedTime { get; set; }
    }
}