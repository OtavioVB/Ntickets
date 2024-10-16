﻿// ------------------------------------------------------------------------------
//  <auto-generated>
//      This code was generated by SpecFlow (https://www.specflow.org/).
//      SpecFlow Version:3.9.0.0
//      SpecFlow Generator Version:3.9.0.0
// 
//      Changes to this file may cause incorrect behavior and will be lost if
//      the code is regenerated.
//  </auto-generated>
// ------------------------------------------------------------------------------
#region Designer generated code
#pragma warning disable
namespace Ntickets.FunctionalTests.UseCases.CreateTenant
{
    using TechTalk.SpecFlow;
    using System;
    using System.Linq;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "3.9.0.0")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public partial class CriacaoDeTenantWhitelabelContratanteFeature : object, Xunit.IClassFixture<CriacaoDeTenantWhitelabelContratanteFeature.FixtureData>, System.IDisposable
    {
        
        private static TechTalk.SpecFlow.ITestRunner testRunner;
        
        private string[] _featureTags = ((string[])(null));
        
        private Xunit.Abstractions.ITestOutputHelper _testOutputHelper;
        
#line 1 "CreateTenant.feature"
#line hidden
        
        public CriacaoDeTenantWhitelabelContratanteFeature(CriacaoDeTenantWhitelabelContratanteFeature.FixtureData fixtureData, Ntickets_FunctionalTests_XUnitAssemblyFixture assemblyFixture, Xunit.Abstractions.ITestOutputHelper testOutputHelper)
        {
            this._testOutputHelper = testOutputHelper;
            this.TestInitialize();
        }
        
        public static void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("pt"), "UseCases/CreateTenant", "Criação de Tenant/Whitelabel (Contratante)", "A short summary of the feature", ProgrammingLanguage.CSharp, ((string[])(null)));
            testRunner.OnFeatureStart(featureInfo);
        }
        
        public static void FeatureTearDown()
        {
            testRunner.OnFeatureEnd();
            testRunner = null;
        }
        
        public virtual void TestInitialize()
        {
        }
        
        public virtual void TestTearDown()
        {
            testRunner.OnScenarioEnd();
        }
        
        public virtual void ScenarioInitialize(TechTalk.SpecFlow.ScenarioInfo scenarioInfo)
        {
            testRunner.OnScenarioInitialize(scenarioInfo);
            testRunner.ScenarioContext.ScenarioContainer.RegisterInstanceAs<Xunit.Abstractions.ITestOutputHelper>(_testOutputHelper);
        }
        
        public virtual void ScenarioStart()
        {
            testRunner.OnScenarioStart();
        }
        
        public virtual void ScenarioCleanup()
        {
            testRunner.CollectScenarioErrors();
        }
        
        void System.IDisposable.Dispose()
        {
            this.TestTearDown();
        }
        
        [Xunit.SkippableTheoryAttribute(DisplayName="Contratante criado com Sucesso")]
        [Xunit.TraitAttribute("FeatureTitle", "Criação de Tenant/Whitelabel (Contratante)")]
        [Xunit.TraitAttribute("Description", "Contratante criado com Sucesso")]
        [Xunit.TraitAttribute("Category", "tenants")]
        [Xunit.InlineDataAttribute("Eventos", "Eventos LTDA", "21330277000152", "eventos-pj@ntickets.com.br", "5511958523443", new string[0])]
        [Xunit.InlineDataAttribute("Eventos", "Otavio Pessoa Fisica", "65360341041", "eventos-pf@ntickets.com.br", "5511958523443", new string[0])]
        public virtual void ContratanteCriadoComSucesso(string fantasyName, string legalName, string document, string email, string phone, string[] exampleTags)
        {
            string[] @__tags = new string[] {
                    "tenants"};
            if ((exampleTags != null))
            {
                @__tags = System.Linq.Enumerable.ToArray(System.Linq.Enumerable.Concat(@__tags, exampleTags));
            }
            string[] tagsOfScenario = @__tags;
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            argumentsOfScenario.Add("FantasyName", fantasyName);
            argumentsOfScenario.Add("LegalName", legalName);
            argumentsOfScenario.Add("Document", document);
            argumentsOfScenario.Add("Email", email);
            argumentsOfScenario.Add("Phone", phone);
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Contratante criado com Sucesso", null, tagsOfScenario, argumentsOfScenario, this._featureTags);
#line 6
this.ScenarioInitialize(scenarioInfo);
#line hidden
            bool isScenarioIgnored = default(bool);
            bool isFeatureIgnored = default(bool);
            if ((tagsOfScenario != null))
            {
                isScenarioIgnored = tagsOfScenario.Where(__entry => __entry != null).Where(__entry => String.Equals(__entry, "ignore", StringComparison.CurrentCultureIgnoreCase)).Any();
            }
            if ((this._featureTags != null))
            {
                isFeatureIgnored = this._featureTags.Where(__entry => __entry != null).Where(__entry => String.Equals(__entry, "ignore", StringComparison.CurrentCultureIgnoreCase)).Any();
            }
            if ((isScenarioIgnored || isFeatureIgnored))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
#line 7
 testRunner.Given(string.Format("um usuário anônimo da plataforma com informações válidas de {0}, {1}, {2}, {3} e " +
                            "{4}", fantasyName, legalName, document, email, phone), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Dado ");
#line hidden
#line 8
 testRunner.When("enviar a solicitação de criação da contratação", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Quando ");
#line hidden
#line 9
 testRunner.Then("a criação do contratante deve ser realizado com sucesso", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Então ");
#line hidden
#line 10
 testRunner.And("o status de utilização deve constar como PENDING_ANALYSIS", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "E ");
#line hidden
#line 11
 testRunner.And("a mensagem de notificação de sucesso \'A criação do whitelabel foi executada com s" +
                        "ucesso.\' com código \'CREATE_TENANT_HAS_BEEN_EXECUTED_SUCCESSFULL\' deve ser retor" +
                        "nada", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "E ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "3.9.0.0")]
        [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
        public class FixtureData : System.IDisposable
        {
            
            public FixtureData()
            {
                CriacaoDeTenantWhitelabelContratanteFeature.FeatureSetup();
            }
            
            void System.IDisposable.Dispose()
            {
                CriacaoDeTenantWhitelabelContratanteFeature.FeatureTearDown();
            }
        }
    }
}
#pragma warning restore
#endregion