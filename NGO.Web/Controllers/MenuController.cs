//using Microsoft.AspNetCore.Mvc;
//using Twilio.AspNet.Core;
//using Twilio.TwiML;
//using Twilio.TwiML.Voice;

//namespace NGO.Web.Controllers
//{
//    [Route("api/v1/Menu")]
//    [ApiController]
//    public class MenuController : TwilioController
//    {
//        [HttpPost]
//        [Route("Welcome")]
//        public async Task<IActionResult> Welcome()
//        {
//            var response = new VoiceResponse();
//            var gather = new Gather(action: new Uri("https://localhost:44326/ShowMenu"), numDigits: 1);
//            gather.Say("Thank you for calling the E.T. Phone Home Service - the " +
//                       "adventurous alien's first choice in intergalactic travel. " +
//                       "Press 1 for directions, press 2 to make a call.");
//            response.Append(gather);

//            return TwiML(response);
//        }
//        [HttpPost]
//        [Route("ShowMenu")]
//        public async Task<IActionResult> Show(string digits)
//        {
//            var selectedOption = digits;
//            var optionActions = new Dictionary<string, Func<ActionResult>>()
//            {
//                //{"1", ReturnInstructions},
//                //{"2", Planets}
//            };
//            var uri = new Uri("https://localhost:44326/ShowMenu");
//            return optionActions.ContainsKey(selectedOption) ?
//                optionActions[selectedOption]() : null;

//        }

//        private async Task<IActionResult> ReturnInstructions()
//        {
//            var response = new VoiceResponse();
//            response.Say("To get to your extraction point, get on your bike and go down " +
//                         "the street. Then Left down an alley. Avoid the police cars. Turn left " +
//                         "into an unfinished housing development. Fly over the roadblock. Go " +
//                         "passed the moon. Soon after you will see your mother ship.",
//                         voice: "alice", language: "en-GB");

//            response.Say("Thank you for calling the E.T. Phone Home Service - the " +
//                         "adventurous alien's first choice in intergalactic travel. Good bye.");

//            response.Hangup();

//            return TwiML(response);
//        }

//        private async Task<IActionResult> Planets()
//        {
//            var response = new VoiceResponse();
//            var gather = new Gather(action: new Uri("PhoneExchange"), numDigits: 1);
//            gather.Say("To call the planet Broh doe As O G, press 2. To call the planet " +
//                     "DuhGo bah, press 3. To call an oober asteroid to your location, press 4. To " +
//                     "go back to the main menu, press the star key ",
//                     voice: "alice", language: "en-GB", loop: 3);
//            response.Append(gather);

//            return TwiML(response);
//        }
//        //[HttpPost]
//        //[Route("PhoneExchange")]
//        //public ActionResult Interconnect(string digits)
//        //{
//        //    var userOption = digits;
//        //    var optionPhones = new Dictionary<string, string>
//        //    {
//        //        {"2", "+19362515374"},
            
//        //    };
//        //    var dial = new Dial(optionPhones[userOption]);
//        //    return optionPhones.ContainsKey(userOption)
//        //        ? dial : RedirectWelcome();
//        //}

//    }
//}
