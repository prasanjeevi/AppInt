﻿// Generated by Xamasoft JSON Class Generator
// http://www.xamasoft.com/json-class-generator

using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Global.Models
{

    public class Test2
    {

        [JsonProperty("_id")]
        public string Id { get; set; }

        [JsonProperty("tenant_id")]
        public int TenantId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("auth_type")]
        public int AuthType { get; set; }

        [JsonProperty("is_sso_enabled")]
        public bool IsSsoEnabled { get; set; }

        [JsonProperty("is_container_sso_agent")]
        public bool IsContainerSsoAgent { get; set; }

        [JsonProperty("provide_container_app")]
        public bool ProvideContainerApp { get; set; }

        [JsonProperty("show_app_within_container")]
        public bool ShowAppWithinContainer { get; set; }

        [JsonProperty("feedback_mail_id")]
        public string FeedbackMailId { get; set; }

        [JsonProperty("exchange_server")]
        public string ExchangeServer { get; set; }

        [JsonProperty("domain_name")]
        public string DomainName { get; set; }

        [JsonProperty("port_no")]
        public int PortNo { get; set; }

        [JsonProperty("auth_url")]
        public string AuthUrl { get; set; }

        [JsonProperty("auth_domains")]
        public string AuthDomains { get; set; }

        [JsonProperty("domain")]
        public string Domain { get; set; }

        [JsonProperty("certificate_name")]
        public string CertificateName { get; set; }

        [JsonProperty("app_wrap_server")]
        public string AppWrapServer { get; set; }

        [JsonProperty("ios_wrapping_details")]
        public IList<IosWrappingDetail> IosWrappingDetails { get; set; }

        [JsonProperty("android_wrapping_details")]
        public IList<AndroidWrappingDetail> AndroidWrappingDetails { get; set; }

        [JsonProperty("sso_details")]
        public IList<SsoDetail> SsoDetails { get; set; }

        [JsonProperty("ldap_url")]
        public string LdapUrl { get; set; }

        [JsonProperty("authentication_priority")]
        public IList<AuthenticationPriority> AuthenticationPriority { get; set; }

        [JsonProperty("external_authenticate_details")]
        public IList<ExternalAuthenticateDetail> ExternalAuthenticateDetails { get; set; }

        [JsonProperty("external_user_details_details")]
        public IList<ExternalUserDetailsDetail> ExternalUserDetailsDetails { get; set; }

        [JsonProperty("external_group_check_details")]
        public IList<ExternalGroupCheckDetail> ExternalGroupCheckDetails { get; set; }

        [JsonProperty("ldap_username")]
        public string LdapUsername { get; set; }

        [JsonProperty("ldap_password")]
        public string LdapPassword { get; set; }

        [JsonProperty("base_dn")]
        public string BaseDn { get; set; }
    }

}