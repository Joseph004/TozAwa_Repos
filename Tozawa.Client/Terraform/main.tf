terraform {
  required_providers {
    aws = {
      source  = "hashicorp/aws"
      version = ">= 3.25.0"
    }
  }
  backend "s3" {
    key = "aws/ec2-deploy/teraform.tfstate"
  }
}

provider "aws" {
  region = var.region
}

resource "azurerm_resource_group" "tozawa_nonprod" {
  name     = "tf_tozawa_nonprod_rg"
  location = "West Europe"
}

resource "aws_instance" "container_nonprod" {
  ami           = "ami-0989fb15ce71ba39e"
  instance_type = "t2.micro"
  key_name      = aws_key_pair.deployer_nonprod.key_name
  vpc_security_group_ids = [aws_security_group.nonprod-group.id]
  iam_instance_profile = aws_iam_instance_profile.ec2-nonprod-profile.name
  connection {
    type        = "ssh"
    host        = self.public_ip
    user        = "ubuntu"
    private_key = var.private_key
    timeout     = "4a"
  }
  tags = {
    "name" = "DeployVM"
  }
}

resource "aws_iam_instance_profile" "ec2-nonprod-profile" {
  name = "ec2-profile"
  role = "EC2-ECR-AUTH-NONPROD"
}

resource "aws_security_group" "nonprod-group" {
  egress = {
    cidr_blocks      = ["0.0.0.0/0"]
    description      = ""
    from_port        = 0
    iov6_cidr_blocks = []
    protocol         = "-1"
    security_groups  = []
    self             = false
    to_port          = 0
  }
  ingress = [
    {
      cidr_blocks      = ["0.0.0.0/0"]
      description      = ""
      from_port        = 22
      iov6_cidr_blocks = []
      protocol         = "tcp"
      security_groups  = []
      self             = false
      to_port          = 22
    },
    {
      cidr_blocks      = ["0.0.0.0/0"]
      description      = ""
      from_port        = 80
      iov6_cidr_blocks = []
      protocol         = "tcp"
      security_groups  = []
      self             = false
      to_port          = 80
    }
  ]
}

resource "aws_key_pair" "deployer_nonprod" {
  key_name   = var.key_name
  public_key = var.public_key
}

output "instance_public_ip" {
  value = aws_instance.container_nonprod.public_ip
  sensitive = true
}
