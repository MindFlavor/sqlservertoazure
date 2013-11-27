using ITPCfSQL.Azure.Management;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITPCfSQL.Azure.CLR.CallbackSupport
{
    internal class DeploymentsPersistentVMRolesWithInputEndpoints
    {
        public Deployment Deployment { get; set; }
        public DeploymentRole DeploymentRole { get; set; }
        public DeploymentRoleConfigurationSetsConfigurationSetInputEndpoint InputEndpoint { get; set; }
    }
}
