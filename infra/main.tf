terraform {
  required_version = ">= 0.14.4"

  required_providers {
    cloudflare = {
      source  = "cloudflare/cloudflare"
      version = "2.17.0"
    }

    aws = {
      source  = "hashicorp/aws"
      version = "3.24.1"
    }
  }

  backend "remote" {
    hostname     = "app.terraform.io"
    organization = "cloverleaf"

    workspaces {
      name = "cloverleaftrack"
    }
  }
}

provider "aws" {
  region = "us-east-2"
}

provider "aws" {
  alias  = "us-east-1"
  region = "us-east-1"
}

provider "cloudflare" {
}

variable "domain_name" {
  description = "The domain name of the website."
  type        = string
  default     = "cloverleaftrack.com"
}

resource "cloudflare_zone" "this" {
  zone = var.domain_name
}

resource "cloudflare_zone_settings_override" "this" {
  zone_id = cloudflare_zone.this.id

  settings {
    always_use_https   = "on"
    brotli             = "on"
    hotlink_protection = "on"
    rocket_loader      = "on"
    websockets         = "off"
    zero_rtt           = "on"

    min_tls_version = "1.3"
    ssl             = "strict"
    tls_1_3         = "zrt"

    minify {
      css  = "on"
      html = "on"
      js   = "on"
    }

    security_header {
      enabled            = "true"
      preload            = "true"
      max_age            = "6"
      include_subdomains = "true"
      nosniff            = "true"
    }
  }
}

resource "cloudflare_record" "www" {
  name    = "www"
  type    = "CNAME"
  zone_id = cloudflare_zone.this.id
  value   = var.domain_name
  proxied = true
}

resource "cloudflare_record" "caa_amazon" {
  name    = var.domain_name
  type    = "CAA"
  zone_id = cloudflare_zone.this.id

  data = {
    flags = "0"
    tag   = "issue"
    value = "amazon.com"
  }
}

resource "cloudflare_record" "caa_amazontrust" {
  name    = var.domain_name
  type    = "CAA"
  zone_id = cloudflare_zone.this.id

  data = {
    flags = "0"
    tag   = "issue"
    value = "amazontrust.com"
  }
}

resource "cloudflare_record" "caa_awstrust" {
  name    = var.domain_name
  type    = "CAA"
  zone_id = cloudflare_zone.this.id

  data = {
    flags = "0"
    tag   = "issue"
    value = "awstrust.com"
  }
}

resource "cloudflare_record" "caa_amazonaws" {
  name    = var.domain_name
  type    = "CAA"
  zone_id = cloudflare_zone.this.id

  data = {
    flags = "0"
    tag   = "issue"
    value = "amazonaws.com"
  }
}

resource "cloudflare_record" "validation" {
  for_each = {
    for dvo in aws_acm_certificate.this.domain_validation_options : dvo.domain_name => {
      name   = dvo.resource_record_name
      record = dvo.resource_record_value
      type   = dvo.resource_record_type
    }
  }

  name    = each.value.name
  value   = trimsuffix(each.value.record, ".")
  type    = each.value.type
  zone_id = cloudflare_zone.this.id
}

resource "cloudflare_record" "cname" {
  count = length(module.cloudfront.cloudfront_domain_names)

  name    = var.domain_name
  value   = module.cloudfront.cloudfront_domain_names[count.index]
  type    = "CNAME"
  zone_id = cloudflare_zone.this.id
}

resource "aws_acm_certificate_validation" "this" {
  provider = aws.us-east-1

  certificate_arn         = aws_acm_certificate.this.arn
  validation_record_fqdns = [for record in cloudflare_record.validation : record.hostname]
}

resource "aws_acm_certificate" "this" {
  provider = aws.us-east-1

  domain_name       = var.domain_name
  validation_method = "DNS"

  tags = {
    Project = "CloverleafTrack"
  }
}

module "static_website" {
  source = "git::git@github.com:gruntwork-io/package-static-assets.git//modules/s3-static-website?ref=v0.7.1"

  website_domain_name = var.domain_name

  custom_tags = {
    Project = "CloverleafTrack"
  }
}

<<<<<<< HEAD
=======
resource "null_resource" "upload" {

  triggers = {
    always_run = timestamp()
  }

  provisioner "local-exec" {
    command = "exec/s3-upload.sh ${module.static_website.website_bucket_name}"
  }
}

>>>>>>> 63ba44575ac5320d8eae014f4a2449682b9f44a9
module "cloudfront" {
  source = "git::git@github.com:gruntwork-io/package-static-assets.git//modules/s3-cloudfront?ref=v0.7.1"

  bucket_name             = var.domain_name
  bucket_website_endpoint = module.static_website.website_bucket_endpoint

  index_document = "index.html"

  s3_bucket_is_public_website = true

  domain_names        = [var.domain_name]
  acm_certificate_arn = aws_acm_certificate.this.arn

  default_ttl = 30
  max_ttl     = 60
  min_ttl     = 0

  custom_tags = {
    Project = "CloverleafTrack"
  }
}
