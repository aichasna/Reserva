"use strict";
jQuery(document).on('ready', function() {
	/* MOBILE MENU*/
	function collapseMenu(){
		jQuery('.at-navdashboard ul li.menu-item-has-children,.at-navigation ul li.menu-item-has-children').prepend('<span class="at-dropdowarrow"><i class="lnr lnr-chevron-right"></i></span>');
		if ($(window).width() < 992) {
			jQuery('.at-navigation ul li.menu-item-has-children a').on('click', function() {
				jQuery(this).parent('li').toggleClass('at-open');
				jQuery(this).next().slideToggle(300);
			});
		}
		jQuery('.at-navdashboard ul li.menu-item-has-children').on('click', function(){
			jQuery(this).toggleClass('at-open');
			jQuery(this).find('.sub-menu').slideToggle(300);
		});
	}
	collapseMenu();
	/* BANNER SLIDER */
	function customerFeedback(){
		var sync1 = jQuery('#at-homeslidervone');
		var sync2 = jQuery('#at-homeslider-thumbnail');
		var slidesPerPage = 3;
		var syncedSecondary = true;
		sync1.owlCarousel({
			items : 1,
			loop: true,
			nav: false,
			dots: false,
			autoplay: true,
			slideSpeed : 2000,
			animateIn: 'fadeIn',
			animateOut: 'fadeOut',
			responsiveRefreshRate : 200,
		}).on('changed.owl.carousel', syncPosition);
		sync2.on('initialized.owl.carousel', function () {
			sync2.find(".owl-item").eq(0).addClass("current");
		})
		.owlCarousel({
			items : slidesPerPage,
			dots: false,
			nav: false,
			margin: 0,
			autoplay: true,
			smartSpeed: 500,
			slideSpeed : 2000,
			slideBy: slidesPerPage,
			responsiveRefreshRate : 100,
			responsiveClass:true,
			responsive:{
				0:{items:1,},
				481:{items:2,},
				1441:{items:3,}
			}
		}).on('changed.owl.carousel', syncPosition2);
		function syncPosition(el) {
			var count = el.item.count-1;
			var current = Math.round(el.item.index - (el.item.count/2) - .5);
			if(current <0) {
				current = count;
			}
			if(current > count) {
				current = 0;
			}
			sync2
			.find(".owl-item")
			.removeClass("current")
			.eq(current)
			.addClass("current")
			var onscreen = sync2.find('.owl-item.active').length - 1;
			var start = sync2.find('.owl-item.active').first().index();
			var end = sync2.find('.owl-item.active').last().index();
			if (current > end) {
				sync2.data('owl.carousel').to(current, 100, true);
			}
			if (current < start) {
				sync2.data('owl.carousel').to(current - onscreen, 100, true);
			}
		}
		function syncPosition2(el) {
			if(syncedSecondary) {
				var number = el.item.index;
				sync1.data('owl.carousel').to(number, 100, true);
			}
		}
		sync2.on("click", ".owl-item", function(e){
			e.preventDefault();
			var number = jQuery(this).index();
			sync1.data('owl.carousel').to(number, 300, true);
		});
	}
	customerFeedback();
     /*TOGGLE DIV */
	jQuery('.at-dropdown').on('click', function(event){
		event.preventDefault();
		var _this = jQuery(this);
		_this.next().slideToggle();
	});
	/* SIDE NAVIGATION */
	var _at_btnopenclose = jQuery('#at-btnopenclose');
	_at_btnopenclose.on('click', function () {
		jQuery('#at-wrapper').toggleClass('at-sidenavshow');
		if( jQuery('#at-wrapper').hasClass('at-sidenavshow') ){
			jQuery('body').addClass('spread-overlay');
			return true;
		}
		jQuery('body').removeClass('spread-overlay');
	});
	var _tg_close = jQuery('#at-closesidebar');
	_tg_close.on('click', function () {
		jQuery('#at-wrapper').toggleClass('at-sidenavshow');
		if( jQuery('#at-wrapper').hasClass('at-sidenavshow') ){
			jQuery('body').addClass('spread-overlay');
			return true;
		}
		jQuery('body').removeClass('spread-overlay');
	});
	/* DROPDOWN RADIO */
	jQuery('input[type=radio]').on('change',
	    function(){
	    	var _this = jQuery(this);
	        var _type = _this.data('title');
	        _this.parents('.at-radioholder').prev('.at-dropdown').find('span .selected-search-type').text(_type);
	});
	/* GUEST DROPDONW */
	jQuery(document).on('click', function(e){
	    var radio_holder = $(".at-radioholder");
	    var at_dropdown = $(".at-dropdown");
	    
   		if (
   			!radio_holder.is(e.target) && radio_holder.has(e.target).length === 0
   			&& !at_dropdown.is(e.target) && at_dropdown.has(e.target).length === 0
   		) 
	    {
	        radio_holder.slideUp(600);
	        e.stopPropagation();
   		}
	});
    /* CALENDAR SLIDER */
	var _at_calendar_slider = jQuery("#at-calendar-slider")
	_at_calendar_slider.owlCarousel({
		items:2,
		nav:false,
		margin:10,
		loop:true,
		dots:false,
		autoplay:false,
		smartSpeed:450,
		navClass: ['at-calendarprev', 'at-calendarnext'],
		navContainerClass: 'at-calendar-nav',
		navText: ['<span class="ti-arrow-left"></span>', '<span class="ti-arrow-right"></span>'],
		responsive:{
			0:{items:1,},
			768:{items:2,}
		}
	});
	jQuery("#at-calendar , .at-calendar").fullCalendar({
        height: "auto",
        dayClick: function(e, a, t) {
            alert("Clicked on: " + e.format())
        }
    });
	/* Featured Properties SLIDER */
	var _at_featured_sliders = jQuery('#at-featured-sliders')
	_at_featured_sliders.owlCarousel({
		items: 3,
		loop:true,
		nav:false,
		margin: 30,
		dots: true,
		autoplay:false,
		autoHeight: true,
		pagination: true,
		responsiveClass:true,
		responsive:{
			0:{items:1,},
			768:{items:2,},
			992:{items:3,}
		}
 	});
	/* Featured Properties SLIDER */
	jQuery(window).load(function() {
  		setTimeout(function(){
  			var _at_featuredslider = jQuery('.at-featuredslider')
				_at_featuredslider.owlCarousel({
					items: 1,
					loop:true,
					dots:false,
					nav:true,
					autoHeight: true,
					margin: 0,
					autoplay:false,
					navClass: ['at-featurprev', 'at-featurnext'],
					navContainerClass: 'at-featur-nav',
					navText: ['<span class="ti-angle-left"></span>', '<span class="ti-angle-right"></span>'],
			 	});
  		}, 30);
	});
 	/* Pretty Photo VIDEO Images */
	jQuery("a[data-rel]").on('each',function () {
		jQuery(this).attr("rel", jQuery(this).data("rel"));
	});
	jQuery("a[data-rel^='prettyPhoto']").prettyPhoto({
		animation_speed: 'normal',
		theme: 'dark_square',
		slideshow: 3000,
		default_width: 800,
        default_height: 500,
        allowfullscreen: true,
		autoplay_slideshow: false,	
		social_tools: false,
		iframe_markup: "<iframe src='{path}' width='{width}' height='{height}' frameborder='no' allowfullscreen='true'></iframe>", 
		deeplinking: false
	});
	/* Testimonials SLIDER */
	jQuery(window).load(function() {
  		setTimeout(function(){
			var _at_testimonials_slider = jQuery('#at-testimonials-slider')
			_at_testimonials_slider.owlCarousel({
				items: 3,
				loop:true,
				nav:false,
				margin: 30,
				dots: true,
				autoplay:false,
				autoHeight: true,
				pagination: true,
				responsiveClass:true,
				responsive:{
					0:{items:1,},
					768:{items:2,},
					992:{items:3,}
				}
		 	});
	 	}, 30);
	});
 	/* COUNTER */
	try {
		var _at_counter_holder = jQuery('#at-counter-holder');
		_at_counter_holder.appear(function () {
			var _at_counter_holder = jQuery('.at-counter-content h3');
			_at_counter_holder.countTo({
				formatter: function (value, options) {
					return value.toFixed(options.decimals).replace(/\B(?=(?:\d{3})+(?!\d))/g, ',');
				}
			});
		});
	} catch (err) {}
	/* AREA RANGE SLIDER */
	if(jQuery('#at-arearangeslider').length > 0){
		jQuery("#at-arearangeslider").slider({
			range: true,
			min: 0,
			max: 1300,
			values: [ 250, 1000 ],
			slide: function( event, ui ) {
				jQuery( ".at-areaangeaft" ).val(ui.values[ 0 ] + ' sq ft' + ' - ' + ui.values[ 1 ] + ' sq ft');
			}
		});
		jQuery( ".at-areaangeaft" ).val(jQuery( "#at-arearangeslider" ).slider( "values", 0 ) + ' sq ft' + ' - ' + jQuery( ".at-arearangeslider" ).slider( "values", 1 ) + ' sq ft');
	}
	/* PRICE RANGE SLIDER */
	if(jQuery('#at-rangeslider').length > 0){
		jQuery("#at-rangeslider").slider({
			range: true,
			min: 0,
			max: 2500,
			values: [ 300, 2000 ],
			slide: function( event, ui ) {
				jQuery( "#at-rangeamount" ).val( "$" + ui.values[ 0 ] + " - $" + ui.values[ 1 ] );
			}
		});
		jQuery( "#at-rangeamount" ).val( "$" + jQuery("#at-rangeslider").slider( "values", 0 ) + " - $" + jQuery("#at-rangeslider").slider( "values", 1 ));
	}
	/* AREA RANGE SLIDER */
	if(jQuery('#at-arearangeslidertwo').length > 0){
		jQuery("#at-arearangeslidertwo").slider({
			range: true,
			min: 0,
			max: 1300,
			values: [ 250, 1000 ],
			slide: function( event, ui ) {
				jQuery( ".at-areaangeafttwo" ).val(ui.values[ 0 ] + ' sq ft' + ' - ' + ui.values[ 1 ] + ' sq ft');
			}
		});
		jQuery( ".at-areaangeafttwo" ).val(jQuery( "#at-arearangeslidertwo" ).slider( "values", 0 ) + ' sq ft' + ' - ' + jQuery( ".at-arearangeslidertwo" ).slider( "values", 1 ) + ' sq ft');
	}
	/* PRICE RANGE SLIDER */
	if(jQuery('#at-rangeslidertwo').length > 0){
		jQuery("#at-rangeslidertwo").slider({
			range: true,
			min: 0,
			max: 2500,
			values: [ 300, 2000 ],
			slide: function( event, ui ) {
				jQuery( "#at-rangeamounttwo" ).val( "$" + ui.values[ 0 ] + " - $" + ui.values[ 1 ] );
			}
		});
		jQuery( "#at-rangeamounttwo" ).val( "$" + jQuery("#at-rangeslidertwo").slider( "values", 0 ) + " - $" + jQuery("#at-rangeslidertwo").slider( "values", 1 ));
	}
	/* ADD AND REMOVE CLASS  */
	if(jQuery('.at-docsearch').length > 0){
		var _dc_docsearch = jQuery('.at-docsearch');
		_dc_docsearch.on('click',function () {
			jQuery(this).parents('.at-innerbanner-holder').addClass('at-open');
			jQuery(this).parents('.at-innerbanner-holder').addClass('at-opensearchs');
		});
		var _at_home = jQuery('.at-home');
		_at_home.on('click',function () {
			jQuery('.at-home').parents('.at-innerbanner-holder').removeClass('at-opensearchs');
		});
	}
	/* Toggle Filter Area */
	$(document).on('click',function(e) {
	    var container = $(".at-advancedsearch-holder");
	    var form_container = $(".at-form-advancedsearch");
	    var btn_filter = $(".at-docsearch");

	    if (!container.is(e.target) 
	    	&& container.has(e.target).length === 0
	    	&& !form_container.is(e.target) && form_container.has(e.target).length === 0
	    	&& !btn_filter.is(e.target) && btn_filter.has(e.target).length === 0
	    ) 
	    {
	        container.slideUp(400);
	        e.stopPropagation();
   		}
	});
	jQuery(document).on('click', '.at-docsearch', function(e){
		e.preventDefault();
		var _this = jQuery(this);
		_this.parents('.at-opensearchs').find('.at-advancedsearch-holder').slideToggle(400);
	});
	/* PRELOADER*/
	jQuery(window).on('load', function() {
		jQuery(".preloader-outer").delay(500).fadeOut();
		jQuery(".at-preloader-holder").delay(200).fadeOut("slow");
	});
	/* SHARES OPTION  */
	jQuery('#at-shareooption').on('click', function() {
		jQuery('.at-tagsshare .at-socialicons').slideToggle('100');
	});
	/* COMINGSOON SLIDER */
	var _at_comingsoonimgslider = jQuery('#at-comingsoonimgslider')
	_at_comingsoonimgslider.owlCarousel({
		items: 1,
		loop:true,
		nav:false,
		margin: 0,
		dots: false,
		autoplay:true,
		touchDrag:false,
		mouseDrag:false,
		animateIn: 'fadeIn',
		animateOut: 'fadeOut',
 	});
 	/* TIPSO TOOLTIP */
	jQuery('.toltip-content').tipso({
			speed             : 400,        
			background        : '#fff',
			titleBackground   : '#3498db',
			color             : '#999999',
			titleColor        : '#ffffff',
			width             : 105,
			tooltipHover      : true,
			size :50,
			offsetY : 0,
			position: 'top-right'
		});
		jQuery('.hover-tipso-tooltip').tipso({
	    tooltipHover: true,
	});
	/* PROPERTY SINGLE SLIDER */
	var _at_propertysilder = jQuery("#at-propertysilder")
	_at_propertysilder.owlCarousel({
		nav:true,
		margin:10,
		loop:true,
		dots:false,
		autoWidth: true,
		autoHeight: true,
		center: true,
		autoplay:false,
		smartSpeed:450
	});
	jQuery('.at-propertysilder .owl-prev').html("<span> <span>PR</span> <span>EV</span> </span>");
	jQuery('.at-propertysilder .owl-next').html("<span> <span>NE</span> <span>XT</span> </span>");
	jQuery('.at-propertysilder .owl-prev span span:last-child,.at-propertysilder .owl-next span span:last-child').addClass("d-block");
	/* SHORT DESCRIPTION */
	var _readmore = jQuery('#at-amenetieslisting');
	_readmore.readmore({
		speed: 500,
		collapsedHeight: 184,
		moreLink: '<div class="at-btnmore"><a href="#">More</a></div>',
		lessLink: '<div class="at-btnmore"><a href="#">Less</a></div>',
	});
	/* SHORT DESCRIPTION */
	var _readmore = jQuery('#at-nearbylocations-holder');
	_readmore.readmore({
		speed: 500,
		collapsedHeight: 556,
		moreLink: '<div class="at-btnmore"><a href="#">More</a></div>',
		lessLink: '<div class="at-btnmore"><a href="#">Less</a></div>',
	});
	/* ADD MORE RADIO SERVICES */
	var _readmore = jQuery('#at-services-checkbox');
	_readmore.readmore({
		speed: 500,
		collapsedHeight: 136,
		moreLink: '<div class="at-showall"><a href="#">show all</a></div>',
		lessLink: '<div class="at-showall"><a href="#">Less</a></div>',
	});
	/* Google Map */
	if(jQuery('#at-locationmap').length > 0){
		var _at_locationmap = jQuery('#at-locationmap');
		_at_locationmap.gmap3({
			marker: {
				address: '1600 Elizabeth St, Melbourne, Victoria, Australia',
				options: {
					title: 'Robert Frost Elementary School'
				}
			},
			map: {
				options: {
					zoom: 16,
					scrollwheel: false,
					disableDoubleClickZoom: true,
				}
			}
		});
	}
	/* DATE PICKER */
	if(document.getElementById("at-startdate") != null){
		var picker = new Lightpick({
		    field: document.getElementById('at-startdate'),
		    secondField: document.getElementById('at-enddate'),
		    singleDate: false,
		    onSelect: function(start, end){
		        var str = '';
		        str += start ? start.format('Do MMMM YYYY') + ' to ' : '';
		        str += end ? end.format('Do MMMM YYYY') : '...';
		    }
		});

	}
	/* DATE PICKER */
	if(document.getElementById("at-startdatetwo") != null){
		var picker = new Lightpick({
		    field: document.getElementById('at-startdatetwo'),
		    secondField: document.getElementById('at-enddatetwo'),
		    singleDate: false,
		    onSelect: function(start, end){
		        var str = '';
		        str += start ? start.format('Do MMMM YYYY') + ' to ' : '';
		        str += end ? end.format('Do MMMM YYYY') : '...';
		    }
		});
	}
});