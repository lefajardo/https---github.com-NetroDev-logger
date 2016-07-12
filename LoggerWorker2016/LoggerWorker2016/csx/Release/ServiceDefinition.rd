<?xml version="1.0" encoding="utf-8"?>
<serviceModel xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" name="LoggerWorker2016" generation="1" functional="0" release="0" Id="8249de27-8ce5-4574-99e3-68713a0eac73" dslVersion="1.2.0.0" xmlns="http://schemas.microsoft.com/dsltools/RDSM">
  <groups>
    <group name="LoggerWorker2016Group" generation="1" functional="0" release="0">
      <componentports>
        <inPort name="WorkerRole1:Microsoft.WindowsAzure.Plugins.RemoteForwarder.RdpInput" protocol="tcp">
          <inToChannel>
            <lBChannelMoniker name="/LoggerWorker2016/LoggerWorker2016Group/LB:WorkerRole1:Microsoft.WindowsAzure.Plugins.RemoteForwarder.RdpInput" />
          </inToChannel>
        </inPort>
      </componentports>
      <settings>
        <aCS name="Certificate|WorkerRole1:Microsoft.WindowsAzure.Plugins.RemoteAccess.PasswordEncryption" defaultValue="">
          <maps>
            <mapMoniker name="/LoggerWorker2016/LoggerWorker2016Group/MapCertificate|WorkerRole1:Microsoft.WindowsAzure.Plugins.RemoteAccess.PasswordEncryption" />
          </maps>
        </aCS>
        <aCS name="WorkerRole1:AzureSqlConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/LoggerWorker2016/LoggerWorker2016Group/MapWorkerRole1:AzureSqlConnectionString" />
          </maps>
        </aCS>
        <aCS name="WorkerRole1:AzureSqlConnectionString14" defaultValue="">
          <maps>
            <mapMoniker name="/LoggerWorker2016/LoggerWorker2016Group/MapWorkerRole1:AzureSqlConnectionString14" />
          </maps>
        </aCS>
        <aCS name="WorkerRole1:BlobRootString" defaultValue="">
          <maps>
            <mapMoniker name="/LoggerWorker2016/LoggerWorker2016Group/MapWorkerRole1:BlobRootString" />
          </maps>
        </aCS>
        <aCS name="WorkerRole1:DataConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/LoggerWorker2016/LoggerWorker2016Group/MapWorkerRole1:DataConnectionString" />
          </maps>
        </aCS>
        <aCS name="WorkerRole1:DiagnosticsConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/LoggerWorker2016/LoggerWorker2016Group/MapWorkerRole1:DiagnosticsConnectionString" />
          </maps>
        </aCS>
        <aCS name="WorkerRole1:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/LoggerWorker2016/LoggerWorker2016Group/MapWorkerRole1:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </maps>
        </aCS>
        <aCS name="WorkerRole1:Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountEncryptedPassword" defaultValue="">
          <maps>
            <mapMoniker name="/LoggerWorker2016/LoggerWorker2016Group/MapWorkerRole1:Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountEncryptedPassword" />
          </maps>
        </aCS>
        <aCS name="WorkerRole1:Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountExpiration" defaultValue="">
          <maps>
            <mapMoniker name="/LoggerWorker2016/LoggerWorker2016Group/MapWorkerRole1:Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountExpiration" />
          </maps>
        </aCS>
        <aCS name="WorkerRole1:Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountUsername" defaultValue="">
          <maps>
            <mapMoniker name="/LoggerWorker2016/LoggerWorker2016Group/MapWorkerRole1:Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountUsername" />
          </maps>
        </aCS>
        <aCS name="WorkerRole1:Microsoft.WindowsAzure.Plugins.RemoteAccess.Enabled" defaultValue="">
          <maps>
            <mapMoniker name="/LoggerWorker2016/LoggerWorker2016Group/MapWorkerRole1:Microsoft.WindowsAzure.Plugins.RemoteAccess.Enabled" />
          </maps>
        </aCS>
        <aCS name="WorkerRole1:Microsoft.WindowsAzure.Plugins.RemoteForwarder.Enabled" defaultValue="">
          <maps>
            <mapMoniker name="/LoggerWorker2016/LoggerWorker2016Group/MapWorkerRole1:Microsoft.WindowsAzure.Plugins.RemoteForwarder.Enabled" />
          </maps>
        </aCS>
        <aCS name="WorkerRole1Instances" defaultValue="[1,1,1]">
          <maps>
            <mapMoniker name="/LoggerWorker2016/LoggerWorker2016Group/MapWorkerRole1Instances" />
          </maps>
        </aCS>
      </settings>
      <channels>
        <lBChannel name="LB:WorkerRole1:Microsoft.WindowsAzure.Plugins.RemoteForwarder.RdpInput">
          <toPorts>
            <inPortMoniker name="/LoggerWorker2016/LoggerWorker2016Group/WorkerRole1/Microsoft.WindowsAzure.Plugins.RemoteForwarder.RdpInput" />
          </toPorts>
        </lBChannel>
        <sFSwitchChannel name="SW:WorkerRole1:Microsoft.WindowsAzure.Plugins.RemoteAccess.Rdp">
          <toPorts>
            <inPortMoniker name="/LoggerWorker2016/LoggerWorker2016Group/WorkerRole1/Microsoft.WindowsAzure.Plugins.RemoteAccess.Rdp" />
          </toPorts>
        </sFSwitchChannel>
      </channels>
      <maps>
        <map name="MapCertificate|WorkerRole1:Microsoft.WindowsAzure.Plugins.RemoteAccess.PasswordEncryption" kind="Identity">
          <certificate>
            <certificateMoniker name="/LoggerWorker2016/LoggerWorker2016Group/WorkerRole1/Microsoft.WindowsAzure.Plugins.RemoteAccess.PasswordEncryption" />
          </certificate>
        </map>
        <map name="MapWorkerRole1:AzureSqlConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/LoggerWorker2016/LoggerWorker2016Group/WorkerRole1/AzureSqlConnectionString" />
          </setting>
        </map>
        <map name="MapWorkerRole1:AzureSqlConnectionString14" kind="Identity">
          <setting>
            <aCSMoniker name="/LoggerWorker2016/LoggerWorker2016Group/WorkerRole1/AzureSqlConnectionString14" />
          </setting>
        </map>
        <map name="MapWorkerRole1:BlobRootString" kind="Identity">
          <setting>
            <aCSMoniker name="/LoggerWorker2016/LoggerWorker2016Group/WorkerRole1/BlobRootString" />
          </setting>
        </map>
        <map name="MapWorkerRole1:DataConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/LoggerWorker2016/LoggerWorker2016Group/WorkerRole1/DataConnectionString" />
          </setting>
        </map>
        <map name="MapWorkerRole1:DiagnosticsConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/LoggerWorker2016/LoggerWorker2016Group/WorkerRole1/DiagnosticsConnectionString" />
          </setting>
        </map>
        <map name="MapWorkerRole1:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/LoggerWorker2016/LoggerWorker2016Group/WorkerRole1/Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </setting>
        </map>
        <map name="MapWorkerRole1:Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountEncryptedPassword" kind="Identity">
          <setting>
            <aCSMoniker name="/LoggerWorker2016/LoggerWorker2016Group/WorkerRole1/Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountEncryptedPassword" />
          </setting>
        </map>
        <map name="MapWorkerRole1:Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountExpiration" kind="Identity">
          <setting>
            <aCSMoniker name="/LoggerWorker2016/LoggerWorker2016Group/WorkerRole1/Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountExpiration" />
          </setting>
        </map>
        <map name="MapWorkerRole1:Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountUsername" kind="Identity">
          <setting>
            <aCSMoniker name="/LoggerWorker2016/LoggerWorker2016Group/WorkerRole1/Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountUsername" />
          </setting>
        </map>
        <map name="MapWorkerRole1:Microsoft.WindowsAzure.Plugins.RemoteAccess.Enabled" kind="Identity">
          <setting>
            <aCSMoniker name="/LoggerWorker2016/LoggerWorker2016Group/WorkerRole1/Microsoft.WindowsAzure.Plugins.RemoteAccess.Enabled" />
          </setting>
        </map>
        <map name="MapWorkerRole1:Microsoft.WindowsAzure.Plugins.RemoteForwarder.Enabled" kind="Identity">
          <setting>
            <aCSMoniker name="/LoggerWorker2016/LoggerWorker2016Group/WorkerRole1/Microsoft.WindowsAzure.Plugins.RemoteForwarder.Enabled" />
          </setting>
        </map>
        <map name="MapWorkerRole1Instances" kind="Identity">
          <setting>
            <sCSPolicyIDMoniker name="/LoggerWorker2016/LoggerWorker2016Group/WorkerRole1Instances" />
          </setting>
        </map>
      </maps>
      <components>
        <groupHascomponents>
          <role name="WorkerRole1" generation="1" functional="0" release="0" software="C:\netro\LoggerWorker2016\LoggerWorker2016\csx\Release\roles\WorkerRole1" entryPoint="base\x64\WaHostBootstrapper.exe" parameters="base\x64\WaWorkerHost.exe " memIndex="-1" hostingEnvironment="consoleroleadmin" hostingEnvironmentVersion="2">
            <componentports>
              <inPort name="Microsoft.WindowsAzure.Plugins.RemoteForwarder.RdpInput" protocol="tcp" />
              <inPort name="Microsoft.WindowsAzure.Plugins.RemoteAccess.Rdp" protocol="tcp" portRanges="3389" />
              <outPort name="WorkerRole1:Microsoft.WindowsAzure.Plugins.RemoteAccess.Rdp" protocol="tcp">
                <outToChannel>
                  <sFSwitchChannelMoniker name="/LoggerWorker2016/LoggerWorker2016Group/SW:WorkerRole1:Microsoft.WindowsAzure.Plugins.RemoteAccess.Rdp" />
                </outToChannel>
              </outPort>
            </componentports>
            <settings>
              <aCS name="AzureSqlConnectionString" defaultValue="" />
              <aCS name="AzureSqlConnectionString14" defaultValue="" />
              <aCS name="BlobRootString" defaultValue="" />
              <aCS name="DataConnectionString" defaultValue="" />
              <aCS name="DiagnosticsConnectionString" defaultValue="" />
              <aCS name="Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="" />
              <aCS name="Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountEncryptedPassword" defaultValue="" />
              <aCS name="Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountExpiration" defaultValue="" />
              <aCS name="Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountUsername" defaultValue="" />
              <aCS name="Microsoft.WindowsAzure.Plugins.RemoteAccess.Enabled" defaultValue="" />
              <aCS name="Microsoft.WindowsAzure.Plugins.RemoteForwarder.Enabled" defaultValue="" />
              <aCS name="__ModelData" defaultValue="&lt;m role=&quot;WorkerRole1&quot; xmlns=&quot;urn:azure:m:v1&quot;&gt;&lt;r name=&quot;WorkerRole1&quot;&gt;&lt;e name=&quot;Microsoft.WindowsAzure.Plugins.RemoteAccess.Rdp&quot; /&gt;&lt;e name=&quot;Microsoft.WindowsAzure.Plugins.RemoteForwarder.RdpInput&quot; /&gt;&lt;/r&gt;&lt;/m&gt;" />
            </settings>
            <resourcereferences>
              <resourceReference name="DiagnosticStore" defaultAmount="[4096,4096,4096]" defaultSticky="true" kind="Directory" />
              <resourceReference name="EventStore" defaultAmount="[1000,1000,1000]" defaultSticky="false" kind="LogStore" />
            </resourcereferences>
            <storedcertificates>
              <storedCertificate name="Stored0Microsoft.WindowsAzure.Plugins.RemoteAccess.PasswordEncryption" certificateStore="My" certificateLocation="System">
                <certificate>
                  <certificateMoniker name="/LoggerWorker2016/LoggerWorker2016Group/WorkerRole1/Microsoft.WindowsAzure.Plugins.RemoteAccess.PasswordEncryption" />
                </certificate>
              </storedCertificate>
            </storedcertificates>
            <certificates>
              <certificate name="Microsoft.WindowsAzure.Plugins.RemoteAccess.PasswordEncryption" />
            </certificates>
          </role>
          <sCSPolicy>
            <sCSPolicyIDMoniker name="/LoggerWorker2016/LoggerWorker2016Group/WorkerRole1Instances" />
            <sCSPolicyUpdateDomainMoniker name="/LoggerWorker2016/LoggerWorker2016Group/WorkerRole1UpgradeDomains" />
            <sCSPolicyFaultDomainMoniker name="/LoggerWorker2016/LoggerWorker2016Group/WorkerRole1FaultDomains" />
          </sCSPolicy>
        </groupHascomponents>
      </components>
      <sCSPolicy>
        <sCSPolicyUpdateDomain name="WorkerRole1UpgradeDomains" defaultPolicy="[5,5,5]" />
        <sCSPolicyFaultDomain name="WorkerRole1FaultDomains" defaultPolicy="[2,2,2]" />
        <sCSPolicyID name="WorkerRole1Instances" defaultPolicy="[1,1,1]" />
      </sCSPolicy>
    </group>
  </groups>
  <implements>
    <implementation Id="ec9a1fed-1854-499b-901e-2b2cb5e0cba6" ref="Microsoft.RedDog.Contract\ServiceContract\LoggerWorker2016Contract@ServiceDefinition">
      <interfacereferences>
        <interfaceReference Id="02e1c86e-fd41-4b6f-ac81-ead6658b19eb" ref="Microsoft.RedDog.Contract\Interface\WorkerRole1:Microsoft.WindowsAzure.Plugins.RemoteForwarder.RdpInput@ServiceDefinition">
          <inPort>
            <inPortMoniker name="/LoggerWorker2016/LoggerWorker2016Group/WorkerRole1:Microsoft.WindowsAzure.Plugins.RemoteForwarder.RdpInput" />
          </inPort>
        </interfaceReference>
      </interfacereferences>
    </implementation>
  </implements>
</serviceModel>