using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Xml.Serialization;

namespace StingrayLoadBalancer
{
    [SettingsSerializeAs(SettingsSerializeAs.Xml)]
    [XmlRoot("StingrayConfiguration")]
    public class Configuration : ApplicationSettingsBase, ICollection<WebFarmSettings>
    {
        private List<WebFarmSettings> inner = new List<WebFarmSettings>();

        public void Add(WebFarmSettings item)
        {
            inner.Add(item);
        }

        public void Clear()
        {
            inner.Clear();
        }

        public bool Contains(WebFarmSettings item)
        {
            return inner.Contains(item);
        }

        public void CopyTo(WebFarmSettings[] array, int arrayIndex)
        {
            inner.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return inner.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(WebFarmSettings item)
        {
            return inner.Remove(item);
        }

        public IEnumerator<WebFarmSettings> GetEnumerator()
        {
            return inner.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return inner.GetEnumerator();
        }
    }

    [SettingsSerializeAs(SettingsSerializeAs.Xml)]
    public class WebFarmSettings 
    {
        [XmlElementAttribute()]
        public string WebFarmName
        {
            get;
            set;
        }

        [XmlElementAttribute()]
        public string PoolName
        {
            get;
            set;
        }

        [XmlElementAttribute()]
        public int TCPPort
        {
            get;
            set;
        }

        [XmlElementAttribute()]
        public string ControlApiUrl
        {
            get;
            set;
        }

        [XmlElementAttribute()]
        public string ControlApiUsername
        {
            get;
            set;
        }

        [XmlElementAttribute()]
        public string ControlApiPassword
        {
            get;
            set;
        }

        [XmlElementAttribute()]
        public int DrainingPeriod
        {
            get;
            set;
        }

        public WebFarmSettings()
        {
            this.TCPPort = 80;
        }
    }
}
