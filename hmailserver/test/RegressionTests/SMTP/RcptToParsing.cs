﻿using System;
using System.Collections.Generic;
using System.Text;
using hMailServer;
using NUnit.Framework;
using RegressionTests.Shared;

namespace RegressionTests.SMTP
{
   [TestFixture]
   class RcptToParsing : TestFixtureBase
   {
      [Test]
      public void RcptToValidAddressShouldSucceed()
      {
         AssertValidMailRcptToCommand("RCPT TO: example@example.com");
      }

      [Test]
      public void MailFromDomainWithDotShouldSucceed()
      {
         AssertValidMailRcptToCommand("RCPT TO: example@exa.mple.com");
      }

      [Test]
      public void RcptToEmptyAddressShouldFail()
      {
         AssertInvalidRcptToCommand("RCPT TO: <>", "550 A valid address is required.");
      }
      
      [Test]
      public void RcptToQuotedAddressWithSpaceShouldSucceed()
      {
         AssertValidMailRcptToCommand("RCPT TO: <\"John Smith\"@example.com>");
      }

      [Test]
      public void RcptToQuotedAddressWithSpaceWithoutGtLtShouldSucceed()
      {
         AssertValidMailRcptToCommand("RCPT TO: \"John Smith\"@example.com");
      }

      [Test]
      public void RcptToQuotedAddressWithoutSpaceShouldFail()
      {
         AssertInvalidRcptToCommand("RCPT TO: <John Smith@example.com>", "550 Invalid syntax. Syntax should be RCPT TO:<mailbox@domain>[crlf]");
      }

      [Test]
      public void RcptToSingleQuoteShouldFail()
      {
         AssertInvalidRcptToCommand("RCPT TO: \"", "550 A valid address is required.");
      }

      [Test]
      public void MailFromWithForwardSlashShouldFail()
      {
         AssertInvalidRcptToCommand("RCPT TO: <example/example@example.com>", "550 A valid address is required.");
      }

      [Test]
      public void MailFromWithBackwardSlashShouldFail()
      {
         AssertInvalidRcptToCommand("RCPT TO: <example\\example@example.com>", "550 A valid address is required.");
      }


      [Test]
      public void RcptToWithSingleGreaterThanShouldFail()
      {
         AssertInvalidRcptToCommand("RCPT TO: <example@example.com", "550 Invalid syntax. Syntax should be RCPT TO:<mailbox@domain>[crlf]");
      }

      [Test]
      public void RcptToWithUnsupporedESMTPExtensionShouldFail()
      {
         AssertInvalidRcptToCommand("RCPT TO: <example@example.com> KEY=VALUE", "550 Unsupported ESMTP extension: KEY=VALUE");
      }



      private void AssertInvalidRcptToCommand(string command, string expectedResponse)
      {
         var smtpClientSimulator = new TcpConnection();
         smtpClientSimulator.Connect(25);
         Assert.IsTrue(smtpClientSimulator.Receive().StartsWith("220"));
         smtpClientSimulator.Send("HELO test\r\n");
         Assert.IsTrue(smtpClientSimulator.Receive().StartsWith("250"));
         smtpClientSimulator.Send("MAIL FROM: <test@test.com>\r\n");
         Assert.IsTrue(smtpClientSimulator.Receive().StartsWith("250"));

         string result = smtpClientSimulator.SendAndReceive(command+ "\r\n");

         smtpClientSimulator.Disconnect();

         Assert.AreEqual(expectedResponse + "\r\n", result);
      }

      private void AssertValidMailRcptToCommand(string comamnd)
      {
         var smtpClientSimulator = new TcpConnection();
         smtpClientSimulator.Connect(25);
         Assert.IsTrue(smtpClientSimulator.Receive().StartsWith("220"));
         smtpClientSimulator.Send("HELO test\r\n");
         Assert.IsTrue(smtpClientSimulator.Receive().StartsWith("250"));
         smtpClientSimulator.Send("MAIL FROM: <test@test.com>\r\n");
         Assert.IsTrue(smtpClientSimulator.Receive().StartsWith("250"));

         string result = smtpClientSimulator.SendAndReceive(comamnd + "\r\n");

         smtpClientSimulator.Disconnect();

         Assert.AreEqual("250 OK\r\n", result);
      }

   }
}
