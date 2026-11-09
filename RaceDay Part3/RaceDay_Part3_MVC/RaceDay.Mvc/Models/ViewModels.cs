using System.ComponentModel.DataAnnotations;
namespace RaceDay.Mvc.Models;
public class LoginVm { [Required,EmailAddress] public string Email {get;set;}=""; [Required,DataType(DataType.Password)] public string Password {get;set;}=""; }
public class RegisterVm { [Required] public string Name {get;set;}=""; [Required,EmailAddress] public string Email {get;set;}=""; [Required,MinLength(8)] public string Password {get;set;}=""; [Required] public string Role {get;set;}="Participant"; }
public class EventVm { public int? Id {get;set;} [Required] public string Name {get;set;}=""; [Required] public string Description {get;set;}=""; [Required] public DateTime Date {get;set;}=DateTime.Today.AddDays(7); [Required] public string Location {get;set;}=""; [Range(0.1,1000)] public decimal Distance {get;set;} [Required] public string EventType {get;set;}="Road"; public IFormFile? Banner {get;set;} }
public class CategoryVm { public int EventId {get;set;} [Required] public string Name {get;set;}=""; [Required] public string Type {get;set;}="Age"; [Range(0,150)] public int Minimum {get;set;} [Range(0,150)] public int Maximum {get;set;} }
public class EnrolVm { public int EventId {get;set;} public int CategoryId {get;set;} }
public class ResultVm { public int EnrolmentId {get;set;} [Required] public string FinishTime {get;set;}=""; [Range(1,100000)] public int Position {get;set;} }
public class ProfileVm { [Required] public string Name {get;set;}=""; [Required,EmailAddress] public string Email {get;set;}=""; public string? Phone {get;set;} public IFormFile? ProfilePicture {get;set;} }
