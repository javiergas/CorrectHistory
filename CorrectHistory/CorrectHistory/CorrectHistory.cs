using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk.Workflow;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Messages;
using System.Activities;

namespace CorrectHistory
{
    public class CorrectHistory : CodeActivity
    {
        [Input("CreatedBy Value")]
        [ReferenceTarget("systemuser")]
        public InArgument<EntityReference> CreatedByValue { get; set; }
        
        [Input("ModifiedBy Value")]
        [ReferenceTarget("systemuser")]
        public InArgument<EntityReference> ModifiedByValue { get; set; }

        [Input("ModifiedOn Value")]
        public InArgument<DateTime> ModifiedOnValue { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            ITracingService tracingService = context.GetExtension<ITracingService>();
            IWorkflowContext WorkflowContext = context.GetExtension<IWorkflowContext>();
            IOrganizationServiceFactory serviceFactory = context.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(WorkflowContext.UserId);

            EntityReference _CreatedByValue = CreatedByValue.Get(context);
            EntityReference _ModifiedByValue = ModifiedByValue.Get(context);
            DateTime _ModifiedOnValue = ModifiedOnValue.Get(context);

            Entity entity = (Entity)WorkflowContext.InputParameters["Target"];

            #region CreatedBy
            if (_CreatedByValue != null)
            {
                tracingService.Trace("Entering CreateBy Zone");

                Entity CreatedByEntity;
                {
                    //Create a request
                    RetrieveRequest retrieveRequest = new RetrieveRequest();
                    retrieveRequest.ColumnSet = new ColumnSet(new string[] { "systemuserid", "fullname" });
                    retrieveRequest.Target = _CreatedByValue;

                    //Execute the request
                    RetrieveResponse retrieveResponse = (RetrieveResponse)service.Execute(retrieveRequest);

                    //Retrieve the Loan Application Entity
                    CreatedByEntity = retrieveResponse.Entity as Entity;

                    tracingService.Trace("NewCreatedBy = " + Convert.ToString(CreatedByEntity.Attributes["fullname"]));   
                }
            }

            Entity updateEntity = new Entity(entity.LogicalName);
            updateEntity["createdby"] = _CreatedByValue;

            //hasta este punto no estaba funcionando ya que la intervención debe realizarse antes de la creación y el custom workflow activity no permite hacerlo de ese modo.
            service.Update(updateEntity);

            #endregion

            #region ModifiedBy
            #endregion

            #region ModifiedOn
            #endregion

        }
    }
}
