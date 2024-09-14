//Create user

var partner = new Partner()
            {
                Email = "tozawango@gmail.com",
                Id = Guid.NewGuid()
            };
            _context.Partners.Add(partner);
            _context.SaveChanges();
            var user1 = new ApplicationUser()
            {
                Email = request.Email,
                UserName = "joseph004",
                UserId = Guid.NewGuid(),
                FirstName = "Joseph",
                LastName = "Luhandu",
                AdminMember = true,
                UserCountry = "Sweden",
                PartnerId = partner.Id,
                Partner = partner
            };
            user1.NormalizedEmail = _normalizer.NormalizeEmail(user1.Email);
            user1.EmailConfirmed = true;
            user1.SecurityStamp = Guid.NewGuid().ToString();
            user1.NormalizedUserName = _normalizer.NormalizeName("joseph004");

            var user2 = new ApplicationUser()
            {
                Email = "arobasjojje@gmail.com",
                UserName = "arobas004",
                UserId = Guid.NewGuid(),
                FirstName = "Joseph",
                LastName = "Luhandu",
                AdminMember = true,
                UserCountry = "Sweden",
                PartnerId = partner.Id,
                Partner = partner
            };
            user2.NormalizedEmail = _normalizer.NormalizeEmail(user2.Email);
            user2.EmailConfirmed = true;
            user2.SecurityStamp = Guid.NewGuid().ToString();
            user2.NormalizedUserName = _normalizer.NormalizeName("arobas004");
            await userManager.CreateAsync(user1, pswd);
            await userManager.CreateAsync(user2, "Zairenumber01!");

// Reset password without requiring current password

 await userManager.RemovePasswordAsync(user);
 await userManager.AddPasswordAsync(user, "newpassword");

// Reset password with current password

 await userManager.ChangePasswordAsync(user, "currentpassword", "newpassord!");