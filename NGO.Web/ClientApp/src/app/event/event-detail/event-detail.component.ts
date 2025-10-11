import { Location } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Store } from '@ngrx/store';
import { MessageService } from 'primeng/api';
import { Observable, Subscribable } from 'rxjs';
import { CommonService } from 'src/app/shared/components/services/common.service';
import { addRegisteredEvent } from 'src/app/store/events/event.actions';
import { selectRegisteredEvents } from 'src/app/store/events/event.selectors';
import { EventService } from '../services/event.service';

@Component({
  selector: 'app-event-detail',
  templateUrl: './event-detail.component.html',
  styleUrls: ['./event-detail.component.css'],
})
export class EventDetailComponent implements OnInit {
  eventData: any;
  dynamicColor: string = 'red';
  userData: any;
  color: string = 'red';
  formattedText: string = '';
  userId!: number;
  orgId!: number;
  isAdmin: boolean = false;
  isRegistered: boolean = false;
  registeredEvents$!: Observable<unknown> | Subscribable<unknown> | Promise<unknown>;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private eventStore: Store,
    private messageService: MessageService,
    private eventService: EventService,
    private commonService: CommonService,
    private location: Location
  ) {}

  ngOnInit(): void {
    const orgId = +this.route.snapshot.paramMap.get('orgId')!;
    const eventId = +this.route.snapshot.paramMap.get('eventId')!;
    this.commonService.getUserData().subscribe({
      next: userData => {
        this.userData = userData;
        this.userId = userData.userId;
        this.orgId = userData.orgId;
      },
      error: () => {},
    });
    if (this.userData && this.userId) {
      this.color = this.userData.organizations[0].BrandColor;
      this.isAdmin = this.userData.roles.some((role: { RoleName: string }) => role.RoleName === 'Admin');

      this.changeColor(this.color);
    }

    this.eventService.getEventById(orgId, eventId).subscribe(data => {
      this.eventData = data; // Ensure eventData is assigned before accessing it

      if (this.eventData && new Date(this.eventData.registrationLastDate) > new Date()) {
        this.eventStore.select(selectRegisteredEvents).subscribe(registeredEventsList => {
          if (registeredEventsList) {
            this.isRegistered = registeredEventsList.includes(eventId);
          }
        });
      } else {
        this.isRegistered = true;
      }
    });
  }

  navigateToEditEvent(eventId: string | number): void {
    this.router.navigate(['/events/edit-event', eventId]);
  }

  ViewRegisteredUsers(eventId: number): void {
    this.router.navigate(['/events/registered-users', eventId]);
  }

  Register(access: string, eventId: number): void {
    if (access === 'Paid') {
      //update the bwlow one
      this.router.navigate(['/events/event-registration', eventId]);
    } else {
      const payload = {
        EventId: eventId,
        UserId: this.userId, // Replace with actual userId
        Access: access,
      };
      this.eventService.registerEvent(this.orgId, payload).subscribe(
        () => {
          this.eventStore.dispatch(addRegisteredEvent({ eventId }));
          this.commonService.showSuccess('Registered Successfully');
        },
        () => {
          this.commonService.showError();
        }
      );
    }
  }
  goBack() {
    this.location.back();
  }
  shareEvent() {
    const eventUrl = window.location.href; // or you can construct the specific event URL dynamically
    const eventTitle = this.eventData?.eventTitle;
    const eventDesc = this.eventData?.eventDesc;

    // Check if the Web Share API is supported
    if (navigator.share) {
      navigator
        .share({
          title: eventTitle,
          text: `Check out this event: ${eventDesc}`,
          url: eventUrl,
        })
        .then(() => {})
        .catch(() => {});
    } else {
      // Fallback to social media share links if Web Share API is not supported
      const shareMessage = `Check out this event: ${eventTitle} - ${eventDesc}`;
      const twitterUrl = `https://twitter.com/intent/tweet?text=${encodeURIComponent(shareMessage)}&url=${encodeURIComponent(eventUrl)}`;
      //const facebookUrl = `https://www.facebook.com/sharer/sharer.php?u=${encodeURIComponent(eventUrl)}`;
      //const linkedInUrl = `https://www.linkedin.com/shareArticle?mini=true&url=${encodeURIComponent(eventUrl)}&title=${encodeURIComponent(eventTitle)}&summary=${encodeURIComponent(eventDesc)}`;

      // Open the sharing dialog based on platform
      // For demonstration, we'll always open Twitter, you can adjust as needed
      const shareWindowUrl = twitterUrl; // Default to Twitter URL

      // You can add more conditions for other platforms as needed, e.g., for Facebook or LinkedIn
      window.open(shareWindowUrl, '_blank');
    }
  }

  changeColor(newColor: string) {
    this.dynamicColor = newColor;
  }

  convertToLocalTime(utcTime: string): string {
    if (!utcTime) return '';

    const utcDate = new Date(utcTime); // Convert the UTC time to a Date object
    return utcDate.toLocaleDateString(); // Convert to local time based on browser's timezone
  }
  formatEventTime(eventTime: string): string {
    if (!eventTime) return '';

    // Split the eventTime into start and end time parts
    const times = eventTime.split('-');
    if (times.length !== 2) return 'Invalid time format';

    const startTime = times[0];
    const endTime = times[1];

    // Convert start and end times into 12-hour format without seconds
    const start = this.convertTo12HourFormat(startTime);
    const end = this.convertTo12HourFormat(endTime);

    // Return the formatted times as "Start Time - End Time"
    return `${start} - ${end}`;
  }

  convertTo12HourFormat(time: string): string {
    const [hours, minutes] = time.split(':').map(num => parseInt(num, 10));

    const date = new Date();
    date.setHours(hours);
    date.setMinutes(minutes);

    // Format the time to 12-hour format without seconds
    return date.toLocaleString('en-US', {
      hour: '2-digit',
      minute: '2-digit',
      hour12: true,
    });
  }

  // Add these utility methods to parse and format text
  formatInputText(inputText: string) {
    return this.commonService.formatText(inputText);
  }
}
