terraform {
  required_version = ">= 0.14.4"
}

provider "aws" {
  region = "us-east-2"
}

module "static_website" {
  source = "git::git@github.com:gruntwork-io/package-static-assets.git//modules/s3-static-website?ref=v0.7.1"

  website_domain_name = "cloverleaftrack.com"
}