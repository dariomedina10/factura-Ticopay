//------------------------------------------------------------------------------
// <auto-generated>
//     Este código fue generado por una herramienta.
//     Versión de runtime:4.0.30319.42000
//
//     Los cambios en este archivo podrían causar un comportamiento incorrecto y se perderán si
//     se vuelve a generar el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TicoPay.ServiceReference {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="CompositeType", Namespace="http://schemas.datacontract.org/2004/07/Its_Integra_Control_FE")]
    [System.SerializableAttribute()]
    public partial class CompositeType : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private bool BoolValueField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string StringValueField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool BoolValue {
            get {
                return this.BoolValueField;
            }
            set {
                if ((this.BoolValueField.Equals(value) != true)) {
                    this.BoolValueField = value;
                    this.RaisePropertyChanged("BoolValue");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string StringValue {
            get {
                return this.StringValueField;
            }
            set {
                if ((object.ReferenceEquals(this.StringValueField, value) != true)) {
                    this.StringValueField = value;
                    this.RaisePropertyChanged("StringValue");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="ServiceReference.ITS_CONTROL_DOCUMENTOS")]
    public interface ITS_CONTROL_DOCUMENTOS {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ITS_CONTROL_DOCUMENTOS/GetData", ReplyAction="http://tempuri.org/ITS_CONTROL_DOCUMENTOS/GetDataResponse")]
        string GetData(int value);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ITS_CONTROL_DOCUMENTOS/GetData", ReplyAction="http://tempuri.org/ITS_CONTROL_DOCUMENTOS/GetDataResponse")]
        System.Threading.Tasks.Task<string> GetDataAsync(int value);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ITS_CONTROL_DOCUMENTOS/GetDataUsingDataContract", ReplyAction="http://tempuri.org/ITS_CONTROL_DOCUMENTOS/GetDataUsingDataContractResponse")]
        TicoPay.ServiceReference.CompositeType GetDataUsingDataContract(TicoPay.ServiceReference.CompositeType composite);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ITS_CONTROL_DOCUMENTOS/GetDataUsingDataContract", ReplyAction="http://tempuri.org/ITS_CONTROL_DOCUMENTOS/GetDataUsingDataContractResponse")]
        System.Threading.Tasks.Task<TicoPay.ServiceReference.CompositeType> GetDataUsingDataContractAsync(TicoPay.ServiceReference.CompositeType composite);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ITS_CONTROL_DOCUMENTOS/RegistrarInformacion", ReplyAction="http://tempuri.org/ITS_CONTROL_DOCUMENTOS/RegistrarInformacionResponse")]
        string RegistrarInformacion(string Datos);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ITS_CONTROL_DOCUMENTOS/RegistrarInformacion", ReplyAction="http://tempuri.org/ITS_CONTROL_DOCUMENTOS/RegistrarInformacionResponse")]
        System.Threading.Tasks.Task<string> RegistrarInformacionAsync(string Datos);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface ITS_CONTROL_DOCUMENTOSChannel : TicoPay.ServiceReference.ITS_CONTROL_DOCUMENTOS, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class TS_CONTROL_DOCUMENTOSClient : System.ServiceModel.ClientBase<TicoPay.ServiceReference.ITS_CONTROL_DOCUMENTOS>, TicoPay.ServiceReference.ITS_CONTROL_DOCUMENTOS {
        
        public TS_CONTROL_DOCUMENTOSClient() {
        }
        
        public TS_CONTROL_DOCUMENTOSClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public TS_CONTROL_DOCUMENTOSClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public TS_CONTROL_DOCUMENTOSClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public TS_CONTROL_DOCUMENTOSClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public string GetData(int value) {
            return base.Channel.GetData(value);
        }
        
        public System.Threading.Tasks.Task<string> GetDataAsync(int value) {
            return base.Channel.GetDataAsync(value);
        }
        
        public TicoPay.ServiceReference.CompositeType GetDataUsingDataContract(TicoPay.ServiceReference.CompositeType composite) {
            return base.Channel.GetDataUsingDataContract(composite);
        }
        
        public System.Threading.Tasks.Task<TicoPay.ServiceReference.CompositeType> GetDataUsingDataContractAsync(TicoPay.ServiceReference.CompositeType composite) {
            return base.Channel.GetDataUsingDataContractAsync(composite);
        }
        
        public string RegistrarInformacion(string Datos) {
            return base.Channel.RegistrarInformacion(Datos);
        }
        
        public System.Threading.Tasks.Task<string> RegistrarInformacionAsync(string Datos) {
            return base.Channel.RegistrarInformacionAsync(Datos);
        }
    }
}
