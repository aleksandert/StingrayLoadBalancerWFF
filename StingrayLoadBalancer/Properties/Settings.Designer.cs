﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18034
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace StingrayLoadBalancer.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "10.0.0.0")]
    public sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(@"
          <StingrayConfiguration xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
            <WebFarmSettings>
              <WebFarmName>LoadGenerator</WebFarmName>
              <PoolName>ServerPool_TestWebServers</PoolName>
              <TCPPort>80</TCPPort>
              <ControlApiUrl>https://sr-si-stmlb01:9090/soap</ControlApiUrl>
              <ControlApiUsername>WFF</ControlApiUsername>
              <ControlApiPassword>G3slo1</ControlApiPassword>
              <DrainingPeriod>600</DrainingPeriod>
            </WebFarmSettings>
          </StingrayConfiguration>
        ")]
        public global::StingrayLoadBalancer.Configuration StingrayConfiguration {
            get {
                return ((global::StingrayLoadBalancer.Configuration)(this["StingrayConfiguration"]));
            }
        }
    }
}
