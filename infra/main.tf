terraform {
  required_version = ">= 0.14.4"

  required_providers {
    cloudflare = {
      source = "cloudflare/cloudflare"
      version = "2.17.0"
    }

    aws = {
      source = "hashicorp/aws"
      version = "3.24.1"
    }
  }

  backend "remote" {
    hostname = "app.terraform.io"
    organization = "cloverleaf"

    workspaces {
      name = "cloverleaftrack"
    }
  }
}

provider "aws" {
  region = "us-east-2"
}

provider "cloudflare" {
}

variable "domain_name" {
  description = "The domain name of the website."
  type = string
  default = "cloverleaftrack.com"
}

resource "cloudflare_zone" "this" {
  zone = var.domain_name
}

resource "cloudflare_zone_dnssec" "this" {
  zone_id = cloudflare_zone.this.id
}

resource "aws_acm_certificate" "this" {
  domain_name = var.domain_name
  validation_method = "DNS"

  tags = {
    Project = "CloverleafTrack"
  }

  lifecycle {
    create_before_destroy = true
  }
}

module "static_website" {
  source = "git::git@github.com:gruntwork-io/package-static-assets.git//modules/s3-static-website?ref=v0.7.1"

  website_domain_name = var.domain_name

  custom_tags = {
    Project = "CloverleafTrack"
  }
}

resource "null_resource" "upload" {
  provisioner "local-exec" {
    command = "exec/s3-upload.sh ${module.static_website.website_bucket_name}"
  }
}

module "cloudfront" {
  source = "git::git@github.com:gruntwork-io/package-static-assets.git//modules/s3-cloudfront?ref=v0.7.1"

  bucket_name             = var.domain_name
  bucket_website_endpoint = module.static_website.website_bucket_endpoint

  index_document = "index.html"

  s3_bucket_is_public_website = true

  default_ttl = 30
  max_ttl     = 60
  min_ttl     = 0

  custom_tags = {
    Project = "CloverleafTrack"
  }
}
